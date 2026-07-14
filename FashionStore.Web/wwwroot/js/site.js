/* =============================================
   FASHIONSTORE - JAVASCRIPT PROFESIONAL
   ============================================= */

// =============================================
// CONFIGURACIÓN DE TOASTR
// =============================================

toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": true,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};

// =============================================
// CONFIGURACIÓN DE SWEETALERT2
// =============================================

const Swal = window.Swal;

window.sweetAlert = {
    success: (title, message = '') => {
        return Swal.fire({
            icon: 'success',
            title: title,
            text: message,
            confirmButtonColor: '#667eea',
            confirmButtonText: 'Aceptar'
        });
    },
    error: (title, message = '') => {
        return Swal.fire({
            icon: 'error',
            title: title,
            text: message,
            confirmButtonColor: '#667eea',
            confirmButtonText: 'Aceptar'
        });
    },
    warning: (title, message = '') => {
        return Swal.fire({
            icon: 'warning',
            title: title,
            text: message,
            confirmButtonColor: '#667eea',
            confirmButtonText: 'Aceptar'
        });
    },
    confirm: (title, message = '', onConfirm = null) => {
        return Swal.fire({
            icon: 'question',
            title: title,
            text: message,
            showCancelButton: true,
            confirmButtonColor: '#667eea',
            cancelButtonColor: '#e2e8f0',
            confirmButtonText: 'Sí, continuar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed && onConfirm) {
                onConfirm();
            }
            return result;
        });
    },
    info: (title, message = '') => {
        return Swal.fire({
            icon: 'info',
            title: title,
            text: message,
            confirmButtonColor: '#667eea',
            confirmButtonText: 'Aceptar'
        });
    }
};

// =============================================
// UTILIDADES GENERALES
// =============================================

window.utils = {
    // Mostrar notificación de éxito
    showSuccess: (message) => {
        toastr.success(message);
    },

    // Mostrar notificación de error
    showError: (message) => {
        toastr.error(message);
    },

    // Mostrar notificación de información
    showInfo: (message) => {
        toastr.info(message);
    },

    // Mostrar notificación de advertencia
    showWarning: (message) => {
        toastr.warning(message);
    },

    // Hacer petición AJAX con manejo de errores
    fetchAPI: async (url, options = {}) => {
        try {
            const defaultOptions = {
                headers: {
                    'Content-Type': 'application/json',
                },
            };

            const response = await fetch(url, { ...defaultOptions, ...options });
            const data = await response.json();

            if (!response.ok) {
                throw new Error(data.message || 'Error en la solicitud');
            }

            return data;
        } catch (error) {
            console.error('Error en fetchAPI:', error);
            window.utils.showError(error.message || 'Error de conexión');
            throw error;
        }
    },

    // Formatear número como moneda
    formatCurrency: (value) => {
        return new Intl.NumberFormat('es-PE', {
            style: 'currency',
            currency: 'PEN'
        }).format(value);
    },

    // Formatear número con separadores
    formatNumber: (value, decimals = 2) => {
        return parseFloat(value).toLocaleString('es-PE', {
            minimumFractionDigits: decimals,
            maximumFractionDigits: decimals
        });
    },

    // Formatear fecha
    formatDate: (date) => {
        return new Date(date).toLocaleDateString('es-PE', {
            year: 'numeric',
            month: 'long',
            day: 'numeric'
        });
    },

    // Formatear fecha y hora
    formatDateTime: (date) => {
        return new Date(date).toLocaleDateString('es-PE', {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    },

    // Validar email
    validateEmail: (email) => {
        const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return regex.test(email);
    },

    // Limpiar formulario
    clearForm: (formId) => {
        const form = document.getElementById(formId);
        if (form) form.reset();
    },

    // Mostrar/Ocultar spinner
    showSpinner: (show = true) => {
        const spinner = document.getElementById('loadingSpinner');
        if (spinner) {
            spinner.style.display = show ? 'flex' : 'none';
        }
    },

    // Deshabilitar/Habilitar botón
    setButtonDisabled: (buttonId, disabled = true) => {
        const button = document.getElementById(buttonId);
        if (button) button.disabled = disabled;
    },

    // Obtener parámetro de URL
    getUrlParameter: (name) => {
        name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
        const regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
        const results = regex.exec(location.search);
        return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
    },

    // Copiar al portapapeles
    copyToClipboard: async (text) => {
        try {
            await navigator.clipboard.writeText(text);
            window.utils.showSuccess('Copiado al portapapeles');
        } catch (error) {
            window.utils.showError('Error al copiar');
        }
    },

    // Generar UUID
    generateUUID: () => {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            const r = Math.random() * 16 | 0;
            const v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
};

// =============================================
// INICIALIZACIÓN DE TOOLTIPS Y POPOVERS
// =============================================

document.addEventListener('DOMContentLoaded', () => {
    // Inicializar tooltips de Bootstrap
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));

    // Inicializar popovers de Bootstrap
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(popoverTriggerEl => new bootstrap.Popover(popoverTriggerEl));

    // Agregar animación de entrada a cards
    const cards = document.querySelectorAll('.card');
    cards.forEach((card, index) => {
        card.style.animation = `fadeInUp 0.5s ease-out ${index * 0.1}s both`;
    });

    // Agregar animación de entrada a elementos con clase animate-fade-in-up
    const fadeElements = document.querySelectorAll('.animate-fade-in-up');
    fadeElements.forEach((el, index) => {
        el.style.animation = `fadeInUp 0.5s ease-out ${index * 0.1}s both`;
    });
});

// =============================================
// FORMULARIOS - VALIDACIÓN
// =============================================

window.formUtils = {
    // Validar formulario Bootstrap
    validateForm: (formId) => {
        const form = document.getElementById(formId);
        if (!form) return true;

        if (!form.checkValidity() === false) {
            event.preventDefault();
            event.stopPropagation();
        }
        form.classList.add('was-validated');
        return form.checkValidity();
    },

    // Mostrar error en campo
    showFieldError: (fieldId, message) => {
        const field = document.getElementById(fieldId);
        if (field) {
            field.classList.add('is-invalid');
            const feedback = field.nextElementSibling;
            if (feedback && feedback.classList.contains('invalid-feedback')) {
                feedback.textContent = message;
            }
        }
    },

    // Limpiar errores del campo
    clearFieldError: (fieldId) => {
        const field = document.getElementById(fieldId);
        if (field) {
            field.classList.remove('is-invalid');
        }
    },

    // Desabilitar todos los campos de un formulario
    disableForm: (formId) => {
        const form = document.getElementById(formId);
        if (form) {
            const inputs = form.querySelectorAll('input, select, textarea, button');
            inputs.forEach(input => input.disabled = true);
        }
    },

    // Habilitar todos los campos de un formulario
    enableForm: (formId) => {
        const form = document.getElementById(formId);
        if (form) {
            const inputs = form.querySelectorAll('input, select, textarea, button');
            inputs.forEach(input => input.disabled = false);
        }
    }
};

// =============================================
// TABLA - UTILIDADES
// =============================================

window.tableUtils = {
    // Inicializar tabla con DataTables (si está cargado)
    initDataTable: (tableId, options = {}) => {
        if (typeof $.fn.dataTable === 'undefined') {
            console.warn('DataTables no está cargado');
            return null;
        }

        const defaultOptions = {
            language: {
                url: '//cdn.datatables.net/plug-ins/1.10.24/i18n/Spanish.json'
            },
            responsive: true,
            autoWidth: false,
            pageLength: 10,
        };

        return $(`#${tableId}`).DataTable({ ...defaultOptions, ...options });
    },

    // Recargar tabla
    reloadTable: (tableId) => {
        if ($.fn.dataTable.isDataTable(`#${tableId}`)) {
            $(`#${tableId}`).DataTable().ajax.reload();
        }
    }
};

// =============================================
// MANEJO DE EVENTOS GLOBALES
// =============================================

// Mostrar/Ocultar spinner en peticiones AJAX
$(document).ajaxStart(() => {
    window.utils.showSpinner(true);
}).ajaxStop(() => {
    window.utils.showSpinner(false);
});

// Manejo de errores globales
window.addEventListener('error', (event) => {
    console.error('Error global:', event.error);
    window.utils.showError('Ocurrió un error inesperado');
});

// Confirmación antes de salir si hay cambios
let hasUnsavedChanges = false;

window.addEventListener('beforeunload', (event) => {
    if (hasUnsavedChanges) {
        event.preventDefault();
        event.returnValue = '';
    }
});

// =============================================
// DARK MODE (Opcional)
// =============================================

window.darkMode = {
    enable: () => {
        document.body.classList.add('dark-mode');
        localStorage.setItem('darkMode', 'true');
    },
    disable: () => {
        document.body.classList.remove('dark-mode');
        localStorage.setItem('darkMode', 'false');
    },
    toggle: () => {
        if (document.body.classList.contains('dark-mode')) {
            window.darkMode.disable();
        } else {
            window.darkMode.enable();
        }
    },
    init: () => {
        if (localStorage.getItem('darkMode') === 'true') {
            window.darkMode.enable();
        }
    }
};

// Inicializar dark mode
window.addEventListener('load', () => {
    window.darkMode.init();
});
