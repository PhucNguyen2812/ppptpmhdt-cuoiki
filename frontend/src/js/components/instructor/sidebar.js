/**
 * Instructor Sidebar Component
 * Renders the navigation sidebar with dark theme for instructor dashboard
 */

export class InstructorSidebar {
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
        id: 'courses',
        label: 'Khóa học',
        icon: this.getIcon('courses'),
        section: 'courses-section'
      },
      {
        id: 'overview',
        label: 'Tổng quan',
        icon: this.getIcon('overview'),
        section: 'overview-section'
      },
      {
        id: 'revenue',
        label: 'Doanh thu',
        icon: this.getIcon('revenue'),
        section: 'revenue-section'
      },
      {
        id: 'students',
        label: 'Học viên',
        icon: this.getIcon('students'),
        section: 'students-section'
      },
      {
        id: 'reviews',
        label: 'Đánh giá',
        icon: this.getIcon('reviews'),
        section: 'reviews-section'
      }
    ];
  }

  getIcon(type) {
    const icons = {
      courses: `
        <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
          <path d="M6.3 2.841A1.5 1.5 0 0 0 4 4.11V15.89a1.5 1.5 0 0 0 2.3 1.269l9.344-5.89a1.5 1.5 0 0 0 0-2.538L6.3 2.84z"/>
        </svg>
      `,
      overview: `
        <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
          <path d="M3 4a1 1 0 0 1 1-1h12a1 1 0 0 1 1 1v2a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V4zM3 10a1 1 0 0 1 1-1h6a1 1 0 0 1 1 1v6a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1v-6zM14 9a1 1 0 0 0-1 1v6a1 1 0 0 0 1 1h2a1 1 0 0 0 1-1v-6a1 1 0 0 0-1-1h-2z"/>
        </svg>
      `,
      revenue: `
        <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
          <path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 0 1-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 0 1-.567.267z"/>
          <path d="M10 18a8 8 0 1 0 0-16 8 8 0 0 0 0 16zm1-13a1 1 0 0 0-1 1v.717a.5.5 0 0 1-.145.35l-.726.726a.5.5 0 0 0-.058.118l-.003.004L9 7.5v.5a.5.5 0 0 0 .5.5h.5a.5.5 0 0 0 .5-.5V8a.5.5 0 0 1 .145-.35l.726-.726a.5.5 0 0 0 .058-.118L11 6.5V6a1 1 0 0 0-1-1z"/>
          <path d="M8 5.5a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-1a.5.5 0 0 1-.5-.5v-1z"/>
        </svg>
      `,
      students: `
        <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
          <path d="M9 6a3 3 0 1 1 6 0 3 3 0 0 1-6 0zM17 16a7 7 0 1 0-14 0h14zM3 6a3 3 0 1 1 6 0 3 3 0 0 1-6 0z"/>
        </svg>
      `,
      reviews: `
        <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
          <path d="M10 15l-5.878 3.09 1.123-6.545L.489 6.91l6.572-.955L10 0l2.939 5.955 6.572.955-4.756 4.635 1.123 6.545z"/>
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

