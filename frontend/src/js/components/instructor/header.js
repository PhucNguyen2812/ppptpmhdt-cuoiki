/**
 * Instructor Header Component
 * Renders the header for instructor dashboard
 */

import { getUserFromToken } from '../../utils/token.js';
import { logout } from '../../api/authApi.js';
import { DEFAULT_IMAGES } from '../../config.js';

export class InstructorHeader {
  constructor(config = {}) {
    this.config = {
      appTitle: config.appTitle || 'udemy',
      showNotifications: config.showNotifications !== false,
      showProfile: config.showProfile !== false,
      onMenuToggle: config.onMenuToggle || null,
      userInfo: config.userInfo || null,
      ...config
    };
    
    // Use provided userInfo or get from token
    this.user = this.config.userInfo || getUserFromToken();
  }

  render() {
    const userInfo = this.user ? {
      name: this.user.name || this.user.hoTen || 'Giảng viên',
      email: this.user.email || '',
      avatar: this.user.anhDaiDien || '',
      role: 'Giảng viên',
      initials: this.getUserInitials(this.user.name || this.user.hoTen || 'Giảng viên')
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
          <div class="instructor-logo-header" id="instructor-logo-header">
            <span>UHIHI</span>
          </div>
        </div>
        
        <div class="header-right">
          <span class="header-text" id="student-link">Học viên</span>
          ${this.config.showProfile ? this.renderUserProfile(userInfo) : ''}
        </div>
      </header>
    `;
  }

  renderUserProfile(userInfo) {
    const avatarUrl = userInfo.avatar || DEFAULT_IMAGES.AVATAR;
    const defaultAvatarSvg = `data:image/svg+xml,%3Csvg xmlns=%22http://www.w3.org/2000/svg%22 width=%2248%22 height=%2248%22%3E%3Crect fill=%22%235624d0%22 width=%2248%22 height=%2248%22/%3E%3Ctext x=%2250%25%22 y=%2250%25%22 font-family=%22Arial%22 font-size=%2220%22 fill=%22%23fff%22 text-anchor=%22middle%22 dy=%22.3em%22%3E${userInfo.initials}%3C/text%3E%3C/svg%3E`;
    
    return `
      <div class="user-profile" id="user-profile-btn" role="button" aria-label="User profile">
        <div class="avatar-wrapper">
          <img src="${avatarUrl}" alt="Avatar" id="user-avatar-img" class="avatar" onerror="this.src='${DEFAULT_IMAGES.AVATAR}'" />
        </div>
        <div class="instructor-dropdown-menu" id="instructor-dropdown-menu">
          <div class="user-info">
            <img src="${avatarUrl}" alt="Avatar" id="dropdown-avatar" onerror="this.src='${defaultAvatarSvg}'" />
            <div>
              <strong id="dropdown-user-name">${userInfo.name}</strong>
              <p id="dropdown-user-email">${userInfo.email || ''}</p>
            </div>
          </div>
          <div class="instructor-dropdown-divider"></div>
          <a href="index.html"><i class="fas fa-home"></i> Trang chủ</a>
          <a href="my-cart.html"><i class="fas fa-shopping-cart"></i> Giỏ hàng của tôi</a>
          <a href="instructor-dashboard.html"><i class="fas fa-chalkboard-teacher"></i> Trang giảng viên</a>
          <div class="instructor-dropdown-divider"></div>
          <a href="#" id="instructor-edit-profile"><i class="fas fa-user-edit"></i> Chỉnh sửa hồ sơ</a>
          <div class="instructor-dropdown-divider"></div>
          <a href="#" id="instructor-logout"><i class="fas fa-sign-out-alt"></i> Đăng xuất</a>
        </div>
      </div>
    `;
  }

  attachEventListeners() {
    const menuBtn = document.getElementById('mobile-menu-btn');
    const userProfileBtn = document.getElementById('user-profile-btn');
    const studentLink = document.getElementById('student-link');
    const logoHeader = document.getElementById('instructor-logo-header');
    const dropdownMenu = document.getElementById('instructor-dropdown-menu');
    const logoutBtn = document.getElementById('instructor-logout');
    const editProfileBtn = document.getElementById('instructor-edit-profile');

    if (menuBtn && this.config.onMenuToggle) {
      menuBtn.addEventListener('click', this.config.onMenuToggle);
    }

    // Logo header - reload instructor dashboard
    if (logoHeader) {
      logoHeader.addEventListener('click', () => {
        window.location.href = 'instructor-dashboard.html';
      });
    }

    // Student link - navigate to home page
    if (studentLink) {
      studentLink.addEventListener('click', () => {
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
        this.handleLogout();
      });
    }

    // Edit profile button
    if (editProfileBtn) {
      editProfileBtn.addEventListener('click', (e) => {
        e.preventDefault();
        // TODO: Navigate to edit profile page
        console.log('Edit profile clicked');
      });
    }
  }

  async handleLogout() {
    if (confirm('Bạn có chắc chắn muốn đăng xuất?')) {
      await logout();
      window.location.href = 'index.html';
    }
  }

  getUserInitials(name) {
    if (!name) return 'U';
    
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

