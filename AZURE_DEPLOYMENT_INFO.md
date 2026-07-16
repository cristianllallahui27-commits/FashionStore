# Azure Deployment - FashionStore

## Status: ✅ Configured and Automated

### Automatic Deployment via GitHub Actions

El proyecto está configurado para despliegue automático a Azure mediante GitHub Actions.

**Archivo de configuración:** `.github/workflows/main_fashionstore.yml`

### Cómo funciona:

1. **Trigger**: Cada push a la rama `main` desencadena automáticamente el workflow
2. **Build**: Se compila el proyecto en .NET 10.x con configuración Release
3. **Publish**: Se genera el artefacto publicado
4. **Deploy**: Se despliega automáticamente a Azure Web App "FashionStore"

### Detalles del Deployment:

- **Azure App Name**: FashionStore
- **Slot**: Production
- **.NET Version**: 10.x (dotnet-latest)
- **Build Configuration**: Release
- **Credentials**: OIDC (OpenID Connect) via GitHub Secrets

### Secrets Configurados en GitHub:

```
AZUREAPPSERVICE_CLIENTID_AD96F8B889FA400789843E1C57ADAB25
AZUREAPPSERVICE_TENANTID_7C6C0D9D2D184E5C8EFCAC435E6EB8B4
AZUREAPPSERVICE_SUBSCRIPTIONID_B7C1AE1F97864FE0A4CFCC0BF23C8035
```

### Estado del Último Push:

- **Commit**: c024d07 (feat: UI improvements - theme customization, image uploads, visible white text, fixed Home navigation)
- **Timestamp**: 2025-07-08
- **Cambios**: 11 files changed, 594 insertions(+)
  - temas.css: Theme system with 6 variants
  - temas.js: Client-side theme management
  - Multiple .cshtml files: Fixed navigation and UI
  - ConfiguracionController.cs: Added GuardarTema endpoint

### Para Monitorear el Deployment:

1. Ve a: https://github.com/cristianllallahui27-commits/FashionStore/actions
2. Busca el workflow "Build and deploy ASP.Net Core app to Azure Web App"
3. Verifica el estado: ✅ Success o ❌ Failed

### Endpoints de Azure (después del deployment):

- **Azure Portal**: https://portal.azure.com
- **App URL**: https://fashionstore.azurewebsites.net (si está configurado)

### Si el Deploy Falla:

1. Revisa los logs en GitHub Actions para errores específicos
2. Verifica que todos los secrets están configurados correctamente
3. Asegúrate de que la aplicación compila localmente: `dotnet build -c Release`

### Cambios UI Deployados:

✅ Sistema de temas personalizables (6 variantes)
✅ Persistencia de temas en localStorage y BD
✅ Funcionalidad de carga de imágenes
✅ Navegación "Inicio" corregida en todas las vistas
✅ Texto blanco visible en tema oscuro

### Próximos Pasos (si es necesario):

- Si necesitas deploy manual, instala Azure CLI: `choco install azure-cli`
- O usa: `az webapp up --runtime dotnet:9.0 --sku B1`
- Para configurar múltiples environments: https://docs.microsoft.com/en-us/azure/app-service/deploy-staging-slots

---

**Deployment Status**: 🟢 Ready for Automatic Deployment
**Last Updated**: 2025-07-08
**Build Status**: ✅ 0 Errors, 285 Tests Passing
