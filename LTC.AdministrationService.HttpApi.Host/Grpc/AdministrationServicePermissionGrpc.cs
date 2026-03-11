//using AdministrationService.Protos;
//using LTC.AdministrationService.Permission;
//using Grpc.Core;
//using LTC.AdministrationService.Permissions;
//using System;
//using System.Threading.Tasks;
//using Volo.Abp;
//using Volo.Abp.Auditing;
//using Volo.Abp.PermissionManagement;

//namespace LTC.AdministrationService.Grpc
//{
//    [DisableAuditing]
//    [IntegrationService]
//    public class AdministrationServicePermissionGrpc(IPermissionAppService permissionAppService) : AdministrationServicePermission.AdministrationServicePermissionBase
//    {
//        public override async Task<IsGrantedPermissionResponse> IsGrantedPermission(IsGrantedPermissionRequest request, ServerCallContext context)
//        {
//            try
//            {
//                var isGranted = await permissionAppService.IsGrantedAsync(request.PermissionName);
//                return new IsGrantedPermissionResponse() { IsGranted = isGranted };
//            }
//            catch
//            {
//                return new IsGrantedPermissionResponse() { IsGranted = false };
//            }
//        }

//        public override async Task<IsGrantedPermissionsResponse> IsGrantedPermissions(IsGrantedPermissionsRequest request, ServerCallContext context)
//        {
//            try
//            {
//                var isGranted = await permissionAppService.IsGrantedMultiplePermissionsAsync([.. request.PermissionNames]);
//                return new IsGrantedPermissionsResponse() { IsGranted = isGranted };
//            }
//            catch
//            {
//                return new IsGrantedPermissionsResponse() { IsGranted = false };
//            }
//        }

//        public override async Task<TestPermissionResponse> TestPermission(TestPermissionRequest request, ServerCallContext context)
//        {
//            try
//            {
//                var permissionName = request.PermissionName;
//                Console.WriteLine($"Testing permission: {permissionName}");
//                return new TestPermissionResponse() { IsGranted = true };
//            }
//            catch (BusinessException ex)
//            {
//                return new TestPermissionResponse() { IsGranted = false };
//            }
//        }
//    }
//}
