using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;

namespace FashionStore.Web.Services
{
    public interface ICarritoServiceWeb
    {
        void AgregarAlCarrito(int prendaId, int cantidad);
        void EliminarDelCarrito(int prendaId);
        void LimpiarCarrito();
        List<Prenda> ObtenerCarrito();
    }

    public class CarritoService : ICarritoServiceWeb
    {
        private const string CARRITO_SESSION_KEY = "Carrito";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public CarritoService(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public void AgregarAlCarrito(int prendaId, int cantidad)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return;

            var carrito = ObtenerCarrito();
            // Implementación simplificada
        }

        public void EliminarDelCarrito(int prendaId)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return;
        }

        public void LimpiarCarrito()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return;
            session.Remove(CARRITO_SESSION_KEY);
        }

        public List<Prenda> ObtenerCarrito()
        {
            return new List<Prenda>();
        }
    }
}
