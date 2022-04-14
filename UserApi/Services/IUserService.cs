namespace UserApi.Services
{
    public interface IUserService
    {
        Task<RegistrationResponse> Register(RegistrationRequest request);
        Task<AuthenticationResponse> Authenticate(AuthenticationRequest request);
        Task<GetDataResponse> GetData(GetDataRequest request);
        Task<SetDataResponse> SetData(SetDataRequest request);
    }
}
