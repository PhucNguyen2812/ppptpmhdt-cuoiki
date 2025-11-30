/**
 * Admin Header Component
 * Renders the header for admin dashboard (similar to InstructorHeader but without "Học viên" button)
 */

import { getUserFromToken } from '../../utils/token.js';
import { logout } from '../../api/authApi.js';
import { getUserInfo } from '../../utils/authHelper.js';
import { DEFAULT_IMAGES } from '../../config.js';

export class AdminHeader {
  constructor(config = {}) {
    this.config = {
      appTitle: config.appTitle || 'UHIHI',
      showNotifications: config.showNotifications !== false,
      showProfile: config.showProfile !== false,
      onMenuToggle: config.onMenuToggle || null,
      userInfo: config.userInfo || null,
      ...config
    };
    
    // Use provided userInfo or get from token
    this.user = this.config.userInfo || getUserFromToken();
  }

  async render() {
    // Try to get user info from API if not provided
    if (!this.config.userInfo && !this.user) {
      try {
        this.user = await getUserInfo();
      } catch (error) {
        console.error('Error loading user info:', error);
      }
    }

    const userInfo = this.user ? {
      name: this.user.name || this.user.hoTen || 'Admin',
      email: this.user.email || '',
      avatar: this.user.anhDaiDien || '',
      role: 'Quản trị viên',
      initials: this.getUserInitials(this.user.name || this.user.hoTen || 'Admin')
    } : {
      name: 'Guest',
      email: '',
      avatar: '',
      role: 'Khách',
      initials: 'G'
    };

    return `
      <header class="header instructor-header">
        <div class="header-left">
          <button class="mobile-menu-btn" id="mobile-menu-btn" aria-label="Toggle menu">
            <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
              <path d="M3 5a1 1 0 0 1 1-1h12a1 1 0 0 1 0 2H4a1 1 0 0 1-1-1zM3 10a1 1 0 0 1 1-1h12a1 1 0 0 1 0 2H4a1 1 0 0 1-1-1zM3 15a1 1 0 0 1 1-1h12a1 1 0 0 1 0 2H4a1 1 0 0 1-1-1z"/>
            </svg>
          </button>
          <div class="instructor-logo-header" id="admin-logo-header">
            <span>${this.config.appTitle}</span>
          </div>
        </div>
        
        <div class="header-right">
          ${this.config.showProfile ? this.renderUserProfile(userInfo) : ''}
        </div>
      </header>
    `;
  }

  renderUserProfile(userInfo) {
    const avatarUrl = userInfo.avatar || DEFAULT_IMAGES.AVATAR;
    
    return `
      <div class="user-profile" id="admin-user-profile-btn" role="button" aria-label="User profile">
        <div class="avatar-wrapper">
          <img src="${avatarUrl}" alt="Avatar" id="admin-user-avatar-img" class="avatar" onerror="this.src='${DEFAULT_IMAGES.AVATAR}'" />
        </div>
        <div class="admin-dropdown-menu" id="admin-dropdown-menu">
          <a href="#" id="admin-logout"><i class="fas fa-sign-out-alt"></i> Đăng xuất</a>
        </div>
      </div>
    `;
  }

  attachEventListeners() {
    const menuBtn = document.getElementById('mobile-menu-btn');
    const userProfileBtn = document.getElementById('admin-user-profile-btn');
    const logoHeader = document.getElementById('admin-logo-header');
    const dropdownMenu = document.getElementById('admin-dropdown-menu');
    const logoutBtn = document.getElementById('admin-logout');

    if (menuBtn && this.config.onMenuToggle) {
      menuBtn.addEventListener('click', this.config.onMenuToggle);
    }

    // Logo header - reload admin dashboard
    if (logoHeader) {
      logoHeader.addEventListener('click', () => {
        window.location.href = 'index.html';
      });
    }

    // User profile dropdown toggle
    if (userProfileBtn && dropdownMenu) {
      userProfileBtn.addEventListener('click', (e) => {
        e.stopPropagation();
        dropdownMenu.classList.toggle('show');
      });
    }

    // Close dropdown when clicking outside
    document.addEventListener('click', (e) => {
      if (dropdownMenu && !userProfileBtn?.contains(e.target)) {
        dropdownMenu.classList.remove('show');
      }
    });

    // Logout button
    if (logoutBtn) {
      logoutBtn.addEventListener('click', (e) => {
        e.preventDefault();
        dropdownMenu.classList.remove('show');
        this.handleLogout();
      });
    }
  }

  async handleLogout() {
    if (confirm('Bạn có chắc chắn muốn đăng xuất?')) {
      await logout();
      window.location.href = '../index.html';
    }
  }

  getUserInitials(name) {
    if (!name) return 'A';
    
    const parts = name.trim().split(' ');
    if (parts.length === 1) {
      return parts[0].charAt(0).toUpperCase();
    }
    
    return (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase();
  }

  destroy() {
    const menuBtn = document.getElementById('mobile-menu-btn');
    if (menuBtn && this.config.onMenuToggle) {
      menuBtn.removeEventListener('click', this.config.onMenuToggle);
    }
  }
}

