using System;
using GymAngel.Domain.Entities;

namespace GymAngel.Business.Services.Email
{
    public static class MembershipEmailTemplates
    {
        // 30-Day Renewal Reminder
        public static string RenewalReminder30Days(User user, MembershipTransaction transaction, string planName)
        {
            var expiryDate = transaction.ExpiryDate.ToString("MMMM dd, yyyy");
            
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Membership Renewal Reminder</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
    <table role='presentation' style='width: 100%; border-collapse: collapse;'>
        <tr>
            <td align='center' style='padding: 40px 0;'>
                <table role='presentation' style='width: 600px; border-collapse: collapse; background-color: #ffffff; box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>
                    
                    <tr>
                        <td style='background: linear-gradient(135deg, #FFD700 0%, #FFA500 100%); padding: 30px; text-align: center;'>
                            <h1 style='margin: 0; color: #000; font-size: 28px;'>💪 GYM ANGEL</h1>
                        </td>
                    </tr>

                    <tr>
                        <td style='padding: 40px 30px;'>
                            <h2 style='color: #333; margin-bottom: 20px;'>Hi {user.FullName},</h2>
                            
                            <p style='color: #666; font-size: 16px; line-height: 1.6;'>
                                Your <strong>{planName}</strong> membership will renew in <strong>30 days</strong> on <strong>{expiryDate}</strong>.
                            </p>

                            <div style='background: #f9f9f9; padding: 20px; border-radius: 8px; margin: 25px 0;'>
                                <h3 style='margin: 0 0 15px 0; color: #FFD700;'>Renewal Details</h3>
                                <p style='margin: 5px 0; color: #666;'><strong>Plan:</strong> {planName}</p>
                                <p style='margin: 5px 0; color: #666;'><strong>Amount:</strong> {FormatPrice(transaction.Amount)}</p>
                                <p style='margin: 5px 0; color: #666;'><strong>Payment Method:</strong> {transaction.PaymentMethod}</p>
                                <p style='margin: 5px 0; color: #666;'><strong>Renewal Date:</strong> {expiryDate}</p>
                            </div>

                            <p style='color: #666; font-size: 16px; line-height: 1.6;'>
                                Your membership will automatically renew 3 days before the expiry date. 
                                You can manage your subscription anytime from your profile.
                            </p>

                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='http://localhost:5034/profile.html' 
                                   style='display: inline-block; padding: 15px 40px; background: linear-gradient(135deg, #FFD700 0%, #FFA500 100%); color: #000; text-decoration: none; border-radius: 8px; font-weight: bold; font-size: 16px;'>
                                    Manage Subscription
                                </a>
                            </div>

                            <p style='color: #999; font-size: 14px; margin-top: 30px;'>
                                Want to upgrade or change your plan? 
                                <a href='http://localhost:5034/pricing.html' style='color: #FFD700; text-decoration: none;'>View all plans</a>
                            </p>
                        </td>
                    </tr>

                    <tr>
                        <td style='padding: 30px; background-color: #333; color: #fff; text-align: center;'>
                            <p style='margin: 0 0 10px 0; font-size: 14px;'>
                                Questions? Contact us at <a href='mailto:support@gymangel.com' style='color: #FFD700; text-decoration: none;'>support@gymangel.com</a>
                            </p>
                            <p style='margin: 0; font-size: 12px; color: #999;'>
                                © 2026 Gym Angel. All rights reserved.
                            </p>
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>
</body>
</html>
";
        }

        // Similar templates for other notifications...
        public static string RenewalSuccess(User user, MembershipTransaction transaction, string planName)
        {
            var newExpiryDate = transaction.ExpiryDate.ToString("MMMM dd, yyyy");
            
            return $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4;'>
    <div style='max-width: 600px; margin: 40px auto; background: white; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>
        
        <div style='background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%); padding: 30px; text-align: center;'>
            <h1 style='margin: 0; color: white; font-size: 32px;'>✓ Renewal Successful!</h1>
        </div>

        <div style='padding: 40px 30px;'>
            <h2 style='color: #333;'>Hi {user.FullName},</h2>
            
            <p style='color: #666; font-size: 16px; line-height: 1.6;'>
                Great news! Your <strong>{planName}</strong> membership has been successfully renewed.
            </p>

            <div style='background: #e8f5e9; padding: 20px; border-radius: 8px; margin: 25px 0; border-left: 4px solid #4CAF50;'>
                <h3 style='margin: 0 0 15px 0; color: #4CAF50;'>Renewal Summary</h3>
                <p style='margin: 5px 0; color: #333;'><strong>Plan:</strong> {planName}</p>
                <p style='margin: 5px 0; color: #333;'><strong>Amount Paid:</strong> {FormatPrice(transaction.Amount)}</p>
                <p style='margin: 5px 0; color: #333;'><strong>New Expiry Date:</strong> {newExpiryDate}</p>
                <p style='margin: 5px 0; color: #333;'><strong>Payment Method:</strong> {transaction.PaymentMethod}</p>
            </div>

            <p style='color: #666; font-size: 16px; line-height: 1.6;'>
                Thank you for continuing your fitness journey with Gym Angel! 
                Your membership is now active until {newExpiryDate}.
            </p>

            <div style='text-align: center; margin: 30px 0;'>
                <a href='http://localhost:5034/profile.html' 
                   style='display: inline-block; padding: 15px 40px; background: linear-gradient(135deg, #FFD700 0%, #FFA500 100%); color: #000; text-decoration: none; border-radius: 8px; font-weight: bold;'>
                    View My Profile
                </a>
            </div>
        </div>

        <div style='padding: 30px; background-color: #333; color: #fff; text-align: center;'>
            <p style='margin: 0; font-size: 12px;'>© 2026 Gym Angel. All rights reserved.</p>
        </div>

    </div>
</body>
</html>
";
        }

        public static string GracePeriodNotice(User user, MembershipTransaction transaction, string planName)
        {
            return $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4;'>
    <div style='max-width: 600px; margin: 40px auto; background: white; border-radius: 10px; overflow: hidden;'>
        
        <div style='background: linear-gradient(135deg, #FFA500 0%, #FF8C00 100%); padding: 30px; text-align: center;'>
            <h1 style='margin: 0; color: white; font-size: 28px;'>⚠️ Membership Expired</h1>
        </div>

        <div style='padding: 40px 30px;'>
            <h2 style='color: #333;'>Hi {user.FullName},</h2>
            
            <p style='color: #666; font-size: 16px; line-height: 1.6;'>
                Your <strong>{planName}</strong> membership has expired. Don't worry - you're in our <strong>7-day grace period</strong>!
            </p>

            <div style='background: #fff3cd; padding: 20px; border-radius: 8px; margin: 25px 0; border-left: 4px solid #FFA500;'>
                <h3 style='margin: 0 0 15px 0; color: #FF8C00;'>Grace Period Details</h3>
                <p style='margin: 5px 0; color: #333;'><strong>Status:</strong> Grace Period (7 days)</p>
                <p style='margin: 5px 0; color: #333;'><strong>Gym Access:</strong> Still Available ✓</p>
                <p style='margin: 5px 0; color: #333;'><strong>Ends:</strong> {transaction.GracePeriodEnd?.ToString("MMMM dd, yyyy")}</p>
            </div>

            <p style='color: #666; font-size: 16px; line-height: 1.6;'>
                You still have access to the gym for the next 7 days. 
                Renew now to continue your membership without interruption!
            </p>

            <div style='text-align: center; margin: 30px 0;'>
                <a href='http://localhost:5034/pricing.html' 
                   style='display: inline-block; padding: 15px 40px; background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%); color: white; text-decoration: none; border-radius: 8px; font-weight: bold; font-size: 18px;'>
                    Renew Membership Now
                </a>
            </div>

            <p style='color: #999; font-size: 14px; text-align: center;'>
                After 7 days, your membership will be suspended and gym access will be revoked.
            </p>
        </div>

        <div style='padding: 30px; background-color: #333; color: #fff; text-align: center;'>
            <p style='margin: 0; font-size: 12px;'>© 2026 Gym Angel</p>
        </div>

    </div>
</body>
</html>
";
        }

        public static string MembershipSuspended(User user, string planName)
        {
            return $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4;'>
    <div style='max-width: 600px; margin: 40px auto; background: white; border-radius: 10px; overflow: hidden;'>
        
        <div style='background: #d32f2f; padding: 30px; text-align: center;'>
            <h1 style='margin: 0; color: white; font-size: 28px;'>Membership Suspended</h1>
        </div>

        <div style='padding: 40px 30px;'>
            <h2 style='color: #333;'>Hi {user.FullName},</h2>
            
            <p style='color: #666; font-size: 16px; line-height: 1.6;'>
                Your grace period has ended and your membership is now <strong>suspended</strong>.
            </p>

            <div style='background: #ffebee; padding: 20px; border-radius: 8px; margin: 25px 0; border-left: 4px solid #d32f2f;'>
                <p style='margin: 5px 0; color: #333;'><strong>Status:</strong> Suspended</p>
                <p style='margin: 5px 0; color: #333;'><strong>Gym Access:</strong> Revoked</p>
                <p style='margin: 5px 0; color: #333;'><strong>Previous Plan:</strong> {planName}</p>
            </div>

            <p style='color: #666; font-size: 16px; line-height: 1.6;'>
                We miss you! Reactivate your membership anytime to get back to your fitness goals.
            </p>

            <div style='text-align: center; margin: 30px 0;'>
                <a href='http://localhost:5034/pricing.html' 
                   style='display: inline-block; padding: 15px 40px; background: linear-gradient(135deg, #FFD700 0%, #FFA500 100%); color: #000; text-decoration: none; border-radius: 8px; font-weight: bold; font-size: 18px;'>
                    Reactivate Membership
                </a>
            </div>

            <p style='background: #e8f5e9; padding: 15px; border-radius: 5px; color: #2e7d32; text-align: center; margin-top: 30px;'>
                <strong>Special Offer:</strong> Come back within 30 days and get 10% off any plan!
            </p>
        </div>

        <div style='padding: 30px; background-color: #333; color: #fff; text-align: center;'>
            <p style='margin: 0; font-size: 12px;'>© 2026 Gym Angel</p>
        </div>

    </div>
</body>
</html>
";
        }

        private static string FormatPrice(decimal price)
        {
            return price.ToString("N0") + " VND";
        }
    }
}
