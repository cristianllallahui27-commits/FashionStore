// =============================================
// GESTOR DE TEMAS - FASHIONSTORE
// ============================================= 

const TemasManager = {
    currentTheme: 'theme-default',
    storageKey: 'fashionstore_theme',
    
    /**
     * Inicializar el gestor de temas
     */
    init: function() {
        this.loadTheme();
        this.createThemeSwitcher();
        this.attachEventListeners();
    },
    
    /**
     * Cargar tema guardado en localStorage
     */
    loadTheme: function() {
        const savedTheme = localStorage.getItem(this.storageKey);
        if (savedTheme) {
            this.setTheme(savedTheme);
        }
    },
    
    /**
     * Establecer un tema
     * @param {string} themeName - Nombre del tema (default, dark, blue, green, orange, red)
     */
    setTheme: function(themeName) {
        // Remover tema actual
        document.body.classList.remove(
            'theme-default',
            'theme-dark',
            'theme-blue',
            'theme-green',
            'theme-orange',
            'theme-red'
        );
        
        // Agregar nuevo tema
        if (themeName !== 'theme-default') {
            document.body.classList.add(themeName);
        }
        
        this.currentTheme = themeName;
        localStorage.setItem(this.storageKey, themeName);
        
        // Guardar en BD (opcional)
        this.saveThemeToDatabase(themeName);
        
        // Actualizar botones activos
        this.updateThemeButtons(themeName);
    },
    
    /**
     * Crear el selector de temas en el navbar
     */
    createThemeSwitcher: function() {
        const navbar = document.querySelector('.navbar');
        if (!navbar) return;
        
        // Buscar si ya existe
        if (document.getElementById('theme-switcher')) return;
        
        const themeSwitcher = document.createElement('div');
        themeSwitcher.id = 'theme-switcher';
        themeSwitcher.className = 'theme-switcher ms-auto d-flex gap-2 align-items-center';
        themeSwitcher.innerHTML = `
            <div class="dropdown">
                <button class="btn btn-sm btn-outline-light dropdown-toggle" 
                        type="button" 
                        id="themeDropdown" 
                        data-bs-toggle="dropdown" 
                        aria-expanded="false">
                    <i class="fas fa-palette me-1"></i>Temas
                </button>
                <ul class="dropdown-menu dropdown-menu-end" aria-labelledby="themeDropdown">
                    <li><a class="dropdown-item theme-option" href="#" data-theme="theme-default">
                        <i class="fas fa-circle me-2" style="color: #667eea;"></i>Púrpura (Predeterminado)
                    </a></li>
                    <li><a class="dropdown-item theme-option" href="#" data-theme="theme-dark">
                        <i class="fas fa-circle me-2" style="color: #1a202c;"></i>Oscuro
                    </a></li>
                    <li><a class="dropdown-item theme-option" href="#" data-theme="theme-blue">
                        <i class="fas fa-circle me-2" style="color: #3b82f6;"></i>Azul
                    </a></li>
                    <li><a class="dropdown-item theme-option" href="#" data-theme="theme-green">
                        <i class="fas fa-circle me-2" style="color: #10b981;"></i>Verde
                    </a></li>
                    <li><a class="dropdown-item theme-option" href="#" data-theme="theme-orange">
                        <i class="fas fa-circle me-2" style="color: #f97316;"></i>Naranja
                    </a></li>
                    <li><a class="dropdown-item theme-option" href="#" data-theme="theme-red">
                        <i class="fas fa-circle me-2" style="color: #ef4444;"></i>Rojo
                    </a></li>
                </ul>
            </div>
        `;
        
        // Insertar antes del cierre de navbar-nav
        const navbarNav = navbar.querySelector('.navbar-nav');
        if (navbarNav && navbarNav.parentElement) {
            navbarNav.parentElement.appendChild(themeSwitcher);
        }
    },
    
    /**
     * Adjuntar event listeners
     */
    attachEventListeners: function() {
        document.addEventListener('click', (e) => {
            if (e.target.closest('.theme-option')) {
                e.preventDefault();
                const theme = e.target.closest('.theme-option').getAttribute('data-theme');
                this.setTheme(theme);
            }
        });
    },
    
    /**
     * Actualizar estado de botones
     */
    updateThemeButtons: function(themeName) {
        document.querySelectorAll('.theme-option').forEach(btn => {
            btn.classList.remove('active');
            if (btn.getAttribute('data-theme') === themeName) {
                btn.classList.add('active');
            }
        });
    },
    
    /**
     * Guardar tema en BD
     */
    saveThemeToDatabase: function(themeName) {
        fetch('/api/configuracion/guardar-tema', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-CSRF-TOKEN': document.querySelector('[name="__RequestVerificationToken"]')?.value || ''
            },
            body: JSON.stringify({
                tema: themeName
            })
        }).catch(err => console.log('Tema guardado localmente'));
    }
};

// Inicializar cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', () => {
    TemasManager.init();
});
