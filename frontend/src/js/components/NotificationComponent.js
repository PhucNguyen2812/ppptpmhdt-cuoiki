/**
 * Notification Component
 * Quản lý hiển thị và tương tác với thông báo
 */

import { getNotifications, getUnreadCount, markAsRead, markAllAsRead } from '../api/notificationApi.js';
import { requireAuth } from '../api/authApi.js';

class NotificationComponent {
  constructor() {
    this.notificationIcon = document.getElementById('notificationIcon');
    this.notificationBadge = document.getElementById('notificationBadge');
    this.notificationDropdown = document.getElementById('notificationDropdown');
    this.notificationList = document.getElementById('notificationList');
    this.markAllReadBtn = document.getElementById('markAllReadBtn');
    this.viewAllNotifications = document.getElementById('viewAllNotifications');
    
    this.isDropdownOpen = false;
    this.notifications = [];
    this.unreadCount = 0;
    this.refreshInterval = null;
    
    this.init();
  }

  async init() {
    // Kiểm tra authentication
    try {
      requireAuth();
    } catch (e) {
      // Nếu chưa đăng nhập, ẩn notification icon
      if (this.notificationIcon) {
        this.notificationIcon.style.display = 'none';
      }
      return;
    }

    if (!this.notificationIcon) return;

    // Setup event listeners
    this.setupEventListeners();
    
    // Load initial data
    await this.loadUnreadCount();
    await this.loadNotifications();
    
    // Auto refresh every 30 seconds
    this.startAutoRefresh();
  }

  setupEventListeners() {
    // Toggle dropdown
    if (this.notificationIcon) {
      this.notificationIcon.addEventListener('click', (e) => {
        e.stopPropagation();
        this.toggleDropdown();
      });
    }

    // Close dropdown when clicking outside
    document.addEventListener('click', (e) => {
      if (this.isDropdownOpen && 
          this.notificationDropdown && 
          !this.notificationDropdown.contains(e.target) &&
          !this.notificationIcon.contains(e.target)) {
        this.closeDropdown();
      }
    });

    // Mark all as read
    if (this.markAllReadBtn) {
      this.markAllReadBtn.addEventListener('click', async (e) => {
        e.stopPropagation();
        await this.handleMarkAllAsRead();
      });
    }

    // View all notifications link
    if (this.viewAllNotifications) {
      this.viewAllNotifications.addEventListener('click', (e) => {
        e.preventDefault();
        // TODO: Navigate to notifications page if exists
        console.log('Navigate to notifications page');
      });
    }
  }

  toggleDropdown() {
    if (this.isDropdownOpen) {
      this.closeDropdown();
    } else {
      this.openDropdown();
    }
  }

  openDropdown() {
    if (this.notificationDropdown) {
      this.notificationDropdown.classList.add('show');
      this.isDropdownOpen = true;
      this.loadNotifications(); // Refresh when opening
    }
  }

  closeDropdown() {
    if (this.notificationDropdown) {
      this.notificationDropdown.classList.remove('show');
      this.isDropdownOpen = false;
    }
  }

  async loadUnreadCount() {
    try {
      this.unreadCount = await getUnreadCount();
      this.updateBadge();
    } catch (error) {
      console.error('Error loading unread count:', error);
    }
  }

  async loadNotifications() {
    if (!this.notificationList) return;

    try {
      this.notificationList.innerHTML = '<div class="notification-loading">Đang tải...</div>';
      
      const result = await getNotifications(null, 1, 10); // Get first 10 notifications
      this.notifications = result.items || [];
      
      this.renderNotifications();
    } catch (error) {
      console.error('Error loading notifications:', error);
      this.notificationList.innerHTML = '<div class="notification-empty">Lỗi khi tải thông báo</div>';
    }
  }

  renderNotifications() {
    if (!this.notificationList) return;

    if (this.notifications.length === 0) {
      this.notificationList.innerHTML = '<div class="notification-empty">Không có thông báo nào</div>';
      return;
    }

    const html = this.notifications.map(notification => {
      const isUnread = notification.trangThai === 'Chưa đọc';
      const timeAgo = this.formatTimeAgo(new Date(notification.ngayTao));
      
      return `
        <div class="notification-item ${isUnread ? 'unread' : ''}" data-id="${notification.id}">
          <div class="notification-item-content">
            <div class="notification-item-title">${this.escapeHtml(notification.tieuDe)}</div>
            <div class="notification-item-body">${this.escapeHtml(notification.noiDung)}</div>
            <div class="notification-item-time">${timeAgo}</div>
          </div>
          ${isUnread ? `
            <div class="notification-item-actions">
              <button class="mark-read-btn" data-id="${notification.id}" title="Đánh dấu đã đọc">
                <i class="fas fa-check"></i>
              </button>
            </div>
          ` : ''}
        </div>
      `;
    }).join('');

    this.notificationList.innerHTML = html;

    // Add event listeners for mark as read buttons
    this.notificationList.querySelectorAll('.mark-read-btn').forEach(btn => {
      btn.addEventListener('click', async (e) => {
        e.stopPropagation();
        const notificationId = parseInt(btn.getAttribute('data-id'));
        await this.handleMarkAsRead(notificationId);
      });
    });

    // Add click handler for notification items
    this.notificationList.querySelectorAll('.notification-item').forEach(item => {
      item.addEventListener('click', (e) => {
        if (e.target.closest('.mark-read-btn')) return; // Don't navigate if clicking mark as read
        
        const notificationId = parseInt(item.getAttribute('data-id'));
        const notification = this.notifications.find(n => n.id === notificationId);
        
        if (notification && notification.idKhoaHoc) {
          // Navigate to course detail if it's a course-related notification
          window.location.href = `course-detail.html?id=${notification.idKhoaHoc}`;
        }
      });
    });
  }

  async handleMarkAsRead(notificationId) {
    try {
      await markAsRead(notificationId);
      
      // Update local state
      const notification = this.notifications.find(n => n.id === notificationId);
      if (notification) {
        notification.trangThai = 'Đã đọc';
        notification.ngayDoc = new Date().toISOString();
      }
      
      // Re-render
      this.renderNotifications();
      
      // Update unread count
      await this.loadUnreadCount();
    } catch (error) {
      console.error('Error marking notification as read:', error);
      alert('Lỗi khi đánh dấu đã đọc');
    }
  }

  async handleMarkAllAsRead() {
    try {
      await markAllAsRead();
      
      // Update local state
      this.notifications.forEach(n => {
        n.trangThai = 'Đã đọc';
        n.ngayDoc = new Date().toISOString();
      });
      
      // Re-render
      this.renderNotifications();
      
      // Update unread count
      await this.loadUnreadCount();
    } catch (error) {
      console.error('Error marking all as read:', error);
      alert('Lỗi khi đánh dấu tất cả đã đọc');
    }
  }

  updateBadge() {
    if (!this.notificationBadge) return;

    if (this.unreadCount > 0) {
      this.notificationBadge.textContent = this.unreadCount > 99 ? '99+' : this.unreadCount;
      this.notificationBadge.style.display = 'flex';
    } else {
      this.notificationBadge.style.display = 'none';
    }
  }

  formatTimeAgo(date) {
    const now = new Date();
    const diff = now - date;
    const seconds = Math.floor(diff / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);

    if (days > 0) {
      return `${days} ngày trước`;
    } else if (hours > 0) {
      return `${hours} giờ trước`;
    } else if (minutes > 0) {
      return `${minutes} phút trước`;
    } else {
      return 'Vừa xong';
    }
  }

  escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
  }

  startAutoRefresh() {
    // Refresh every 30 seconds
    this.refreshInterval = setInterval(async () => {
      await this.loadUnreadCount();
      if (this.isDropdownOpen) {
        await this.loadNotifications();
      }
    }, 30000);
  }

  destroy() {
    if (this.refreshInterval) {
      clearInterval(this.refreshInterval);
    }
  }
}

// Export singleton instance
let notificationComponentInstance = null;

export function initNotificationComponent() {
  if (!notificationComponentInstance) {
    notificationComponentInstance = new NotificationComponent();
  }
  return notificationComponentInstance;
}

export default NotificationComponent;




