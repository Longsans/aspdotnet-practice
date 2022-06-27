namespace Practice.ViewModels
{
    public class AccountSettingsViewModel
    {
        public UserInfoViewModel? UserInfo { get; set; }
        public ChangePasswordViewModel? ChangePassword { get; set; }

        public AccountSettingsViewModel() { }
        public AccountSettingsViewModel(UserInfoViewModel userInfo, ChangePasswordViewModel changePassword)
        {
            UserInfo = userInfo;
            ChangePassword = changePassword;
        }
        public AccountSettingsViewModel(UserInfoViewModel userInfo)
        {
            UserInfo = userInfo;
        }

        public AccountSettingsViewModel(ChangePasswordViewModel changePassword)
        {
            ChangePassword = changePassword;
        }
    }
}
