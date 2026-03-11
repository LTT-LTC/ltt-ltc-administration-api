using LTC.AdministrationService.Entities.Mongo;
using LTC.CustomerManagement.Settings;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace LTC.AdministrationService.Data;

public class MailTemplateDataSeeder : IDataSeedContributor, ITransientDependency
{
    private readonly IMailTemplateRepository _mailTemplateRepository;

    public MailTemplateDataSeeder(IMailTemplateRepository mailTemplateRepository)
    {
        _mailTemplateRepository = mailTemplateRepository;
    }

    public virtual async Task SeedAsync(DataSeedContext context)
    {
        // Seed PASSWORD_RECOVERY template
        if (!await _mailTemplateRepository.AnyAsync(x => x.Code == "PASSWORD_RECOVERY"))
        {
            await _mailTemplateRepository.InsertAsync(new MailTemplate
            {
                Code = "PASSWORD_RECOVERY",
                Name = "Yêu cầu khôi phục mật khẩu",
                Subject = "Yêu cầu khôi phục mật khẩu - IT-Zone",
                Body = """
                    <div style="background-color: #eff2f7; font-family: sans-serif; padding: 20px; min-height: 100vh;">
                        <div style="max-width: 600px; margin: 0 auto;">
                            <div style="text-align: center; margin-bottom: 30px;">
                                <img src="https://localhost:4200/images/main/logo_ITZone.png" 
                                    alt="IT-Zone Logo" 
                                    style="border-radius: 12px; width: 100px; height: 100px; display: block; margin: 0 auto;">
                            </div>

                            <div style="background-color: #ffffff; border-radius: 16px; padding: 40px; box-shadow: 0 1px 3px rgba(0,0,0,0.1);">
                                <h1 style="font-size: 18px; font-weight: bold; color: #333333; margin-top: 0; margin-bottom: 20px;">
                                    Hi ##Name##,
                                </h1>

                                <p style="font-size: 16px; line-height: 26px; color: #333333; margin-bottom: 25px;">
                                    Nhấn vào nút dưới đây để di chuyển tới trang Khôi phục mật khẩu. 
                                    Hãy nhớ rằng đường dẫn này sẽ hết hạn sau <strong>##ExpireHours##</strong> giờ.
                                </p>

                                <div style="text-align: center; margin: 35px 0;">
                                    <a href="##ResetLink##" 
                                    style="background-color: #295DFA; color: #ffffff; font-size: 16px; font-weight: 600; text-decoration: none; padding: 14px 24px; border-radius: 8px; display: inline-block; box-shadow: 0 4px 6px rgba(41, 93, 250, 0.2);">
                                        Di chuyển tới trang Khôi phục mật khẩu
                                    </a>
                                </div>

                                <p style="font-size: 16px; line-height: 26px; color: #333333; margin-bottom: 20px;">
                                    Nếu không có ý định đổi mật khẩu thì hãy bỏ qua Email này.
                                </p>

                                <p style="font-size: 16px; line-height: 26px; color: #333333; margin-top: 30px; margin-bottom: 30px;">
                                    Trân trọng,<br />
                                    <strong>IT-Zone.</strong>
                                </p>

                                <p style="font-size: 13px; color: #666666; border-top: 1px solid #eaeaea; padding-top: 20px; line-height: 20px;">
                                    <strong>Lưu ý:</strong> Đây là email tự động, vui lòng không trả lời email này.
                                </p>
                            </div>
                        </div>
                    </div>
                    """,
                Description = "Email template for password recovery feature"
            });
        }

        // Seed PASSWORD_RECOVERY_SUCCESS template
        if (!await _mailTemplateRepository.AnyAsync(x => x.Code == "PASSWORD_RECOVERY_SUCCESS"))
        {
            await _mailTemplateRepository.InsertAsync(new MailTemplate
            {
                Code = "PASSWORD_RECOVERY_SUCCESS",
                Name = "Thông báo khôi phục mật khẩu thành công",
                Subject = "Mật khẩu đã được khôi phục thành công - ##UserName##",
                Body = """
                    <div style="background-color: #eff2f7; font-family: sans-serif; padding: 20px;">
                        <div style="max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 16px; padding: 40px; box-shadow: 0 1px 3px rgba(0,0,0,0.1);">
                            <h1 style="font-size: 18px; font-weight: bold; color: #333333; margin-top: 0;">Hi ##Name##,</h1>
                            
                            <p style="font-size: 16px; line-height: 26px; color: #333333;">
                                Tài khoản hệ thống của <strong>##UserName##</strong> đã được khôi phục thành công vào lúc <strong>##Time##</strong> ngày <strong>##Date##</strong>.
                            </p>

                            <p style="font-size: 16px; line-height: 26px; color: #333333;">
                                Nếu việc khôi phục mật khẩu của tài khoản này không phải do bạn, hãy liên hệ với System Admin ngay lập tức.
                            </p>

                            <p style="font-size: 16px; line-height: 26px; color: #333333; margin-top: 30px;">
                                Trân trọng,<br />
                                <strong>IT-Zone.</strong>
                            </p>
                            
                            <p style="font-size: 13px; color: #666666; border-top: 1px solid #eaeaea; padding-top: 20px; margin-top: 20px;">
                                <strong>Lưu ý:</strong> Đây là email tự động, vui lòng không trả lời email này.
                            </p>
                        </div>
                    </div>
                    """,
                Description = "Email template for successful password reset notification"
            });
        }
    }
}