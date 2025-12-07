/**
 * Admin Sidebar Component
 * Renders the navigation sidebar with dark theme for admin dashboard
 * Menu items: Overview, Users, Course Review, Categories
 */

export class AdminSidebar {
  constructor(config = {}) {
    this.config = {
      menuItems: config.menuItems || this.getDefaultMenuItems(),
      activeItem: config.activeItem || 'overview',
      onItemClick: config.onItemClick || null,
      ...config
    };
  }

  getDefaultMenuItems() {
    return [
      {
        id: 'overview',
        label: 'Tổng quan',
        icon: this.getIcon('overview'),
        section: 'overview-section'
      },
      {
        id: 'users',
        label: 'Quản lý người dùng',
        icon: this.getIcon('users'),
        section: 'users-section'
      },
      {
        id: 'instructor-requests',
        label: 'Kiểm duyệt đăng ký giảng viên',
        icon: this.getIcon('instructor-requests'),
        section: 'instructor-requests-section'
      },
      {
        id: 'categories',
        label: 'Quản lý danh mục',
        icon: this.getIcon('categories'),
        section: 'categories-section'
      }
    ];
  }

  getIcon(type) {
    const icons = {
      overview: `
        <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
          <path d="M3 4a1 1 0 0 1 1-1h12a1 1 0 0 1 1 1v2a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V4zM3 10a1 1 0 0 1 1-1h6a1 1 0 0 1 1 1v6a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1v-6zM14 9a1 1 0 0 0-1 1v6a1 1 0 0 0 1 1h2a1 1 0 0 0 1-1v-6a1 1 0 0 0-1-1h-2z"/>
        </svg>
      `,
      users: `
        <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
          <path d="M9 6a3 3 0 1 1 6 0 3 3 0 0 1-6 0zM17 16a7 7 0 1 0-14 0h14zM3 6a3 3 0 1 1 6 0 3 3 0 0 1-6 0z"/>
        </svg>
      `,
      categories: `
        <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
          <path d="M3 4a1 1 0 0 1 1-1h12a1 1 0 0 1 1 1v2a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V4zM3 10a1 1 0 0 1 1-1h12a1 1 0 0 1 1 1v6a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1v-6z"/>
        </svg>
      `,
      'instructor-requests': `
        <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
          <path d="M9 6a3 3 0 116 0 3 3 0 01-6 0zM17 16a7 7 0 11-14 0h14zM12.293 7.293a1 1 0 011.414 0l3 3a1 1 0 01-1.414 1.414L13 9.414V13a1 1 0 11-2 0V9.414l-1.293 1.293a1 1 0 01-1.414-1.414l3-3z"/>
        </svg>
      `
    };

    return icons[type] || icons.overview;
  }

  render() {
    return `
      <aside class="sidebar instructor-sidebar" id="sidebar">
        <nav class="nav-menu">
          ${this.config.menuItems.map(item => this.renderMenuItem(item)).join('')}
        </nav>
      </aside>
    `;
  }

  renderMenuItem(item) {
    const isActive = item.id === this.config.activeItem;
    
    return `
      <div class="nav-item ${isActive ? 'active' : ''}" data-nav-id="${item.id}" data-section="${item.section}">
        <div class="nav-icon">
          ${item.icon}
        </div>
        <span>${item.label}</span>
      </div>
    `;
  }

  attachEventListeners() {
    const navItems = document.querySelectorAll('.nav-item');
    
    navItems.forEach(item => {
      item.addEventListener('click', (e) => {
        const navId = e.currentTarget.dataset.navId;
        const section = e.currentTarget.dataset.section;
        
        navItems.forEach(nav => nav.classList.remove('active'));
        e.currentTarget.classList.add('active');
        
        if (this.config.onItemClick) {
          this.config.onItemClick(navId, section);
        }
      });
    });
  }

  setActiveItem(itemId) {
    this.config.activeItem = itemId;
    
    const navItems = document.querySelectorAll('.nav-item');
    navItems.forEach(item => {
      if (item.dataset.navId === itemId) {
        item.classList.add('active');
      } else {
        item.classList.remove('active');
      }
    });
  }

  destroy() {
    // Cleanup event listeners
  }
}






