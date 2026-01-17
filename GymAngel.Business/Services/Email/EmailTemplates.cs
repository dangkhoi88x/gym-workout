using System;
using System.Text;
using GymAngel.Domain.Entities;

namespace GymAngel.Business.Services.Email
{
    public static class EmailTemplates
    {
        public static string OrderConfirmation(Domain.Entities.Order order, Domain.Entities.User user)
        {
            var itemsHtml = new StringBuilder();
            foreach (var item in order.OrderItems ?? new System.Collections.Generic.List<Domain.Entities.OrderItem>())
            {
                itemsHtml.Append($@"
                    <tr>
                        <td style='padding: 12px; border-bottom: 1px solid #eee;'>{item.ProductName}</td>
                        <td style='padding: 12px; border-bottom: 1px solid #eee; text-align: center;'>{item.Quantity}</td>
                        <td style='padding: 12px; border-bottom: 1px solid #eee; text-align: right;'>{FormatPrice(item.UnitPrice)}</td>
                        <td style='padding: 12px; border-bottom: 1px solid #eee; text-align: right;'>{FormatPrice(item.UnitPrice * item.Quantity)}</td>
                    </tr>
                ");
            }

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Order Confirmation - Gym Angel</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
    <table role='presentation' style='width: 100%; border-collapse: collapse;'>
        <tr>
            <td align='center' style='padding: 40px 0;'>
                <table role='presentation' style='width: 600px; border-collapse: collapse; background-color: #ffffff; box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>
                    
                    <!-- Header -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #FFD700 0%, #FFA500 100%); padding: 30px; text-align: center;'>
                            <h1 style='margin: 0; color: #000; font-size: 28px; font-weight: bold;'>
                                💪 GYM ANGEL
                            </h1>
                            <p style='margin: 10px 0 0 0; color: #333; font-size: 14px;'>Your Fitness Partner</p>
                        </td>
                    </tr>

                    <!-- Success Message -->
                    <tr>
                        <td style='padding: 30px; text-align: center; background-color: #f9f9f9;'>
                            <div style='background-color: #4CAF50; color: white; padding: 15px; border-radius: 8px; display: inline-block;'>
                                <h2 style='margin: 0; font-size: 24px;'>✓ Order Confirmed!</h2>
                            </div>
                            <p style='margin: 20px 0 0 0; color: #666; font-size: 16px;'>
                                Thank you for your order, <strong>{user.FullName}</strong>!
                            </p>
                            <p style='margin: 5px 0 0 0; color: #999; font-size: 14px;'>
                                We've received your order and will process it shortly.
                            </p>
                        </td>
                    </tr>

                    <!-- Order Details -->
                    <tr>
                        <td style='padding: 30px;'>
                            <h3 style='margin: 0 0 20px 0; color: #333; border-bottom: 2px solid #FFD700; padding-bottom: 10px;'>
                                Order Details
                            </h3>
                            
                            <table style='width: 100%; margin-bottom: 20px;'>
                                <tr>
                                    <td style='padding: 8px 0; width: 40%; color: #666;'>Order Number:</td>
                                    <td style='padding: 8px 0; color: #333; font-weight: bold;'>{order.OrderNumber}</td>
                                </tr>
                                <tr>
                                    <td style='padding: 8px 0; color: #666;'>Order Date:</td>
                                    <td style='padding: 8px 0; color: #333;'>{order.OrderDate:MMMM dd, yyyy HH:mm}</td>
                                </tr>
                                <tr>
                                    <td style='padding: 8px 0; color: #666;'>Payment Method:</td>
                                    <td style='padding: 8px 0; color: #333;'>{order.PaymentMethod}</td>
                                </tr>
                                <tr>
                                    <td style='padding: 8px 0; color: #666;'>Status:</td>
                                    <td style='padding: 8px 0;'>
                                        <span style='background-color: #FFF3CD; color: #856404; padding: 4px 12px; border-radius: 12px; font-size: 12px; font-weight: bold;'>
                                            {order.Status}
                                        </span>
                                    </td>
                                </tr>
                            </table>

                            <h3 style='margin: 30px 0 20px 0; color: #333; border-bottom: 2px solid #FFD700; padding-bottom: 10px;'>
                                Delivery Information
                            </h3>
                            
                            <table style='width: 100%; margin-bottom: 20px;'>
                                <tr>
                                    <td style='padding: 8px 0; width: 40%; color: #666;'>Receiver:</td>
                                    <td style='padding: 8px 0; color: #333;'>{order.ReceiverName}</td>
                                </tr>
                                <tr>
                                    <td style='padding: 8px 0; color: #666;'>Phone:</td>
                                    <td style='padding: 8px 0; color: #333;'>{order.ReceiverPhone}</td>
                                </tr>
                                <tr>
                                    <td style='padding: 8px 0; color: #666; vertical-align: top;'>Address:</td>
                                    <td style='padding: 8px 0; color: #333;'>
                                        {order.DeliveryAddress}<br>
                                        {order.Ward}, {order.District}, {order.City}
                                    </td>
                                </tr>
                                {(string.IsNullOrEmpty(order.Notes) ? "" : $@"
                                <tr>
                                    <td style='padding: 8px 0; color: #666; vertical-align: top;'>Notes:</td>
                                    <td style='padding: 8px 0; color: #333;'>{order.Notes}</td>
                                </tr>
                                ")}
                            </table>

                            <h3 style='margin: 30px 0 20px 0; color: #333; border-bottom: 2px solid #FFD700; padding-bottom: 10px;'>
                                Order Items
                            </h3>
                            
                            <table style='width: 100%; border-collapse: collapse;'>
                                <thead>
                                    <tr style='background-color: #f9f9f9;'>
                                        <th style='padding: 12px; text-align: left; border-bottom: 2px solid #ddd;'>Product</th>
                                        <th style='padding: 12px; text-align: center; border-bottom: 2px solid #ddd;'>Qty</th>
                                        <th style='padding: 12px; text-align: right; border-bottom: 2px solid #ddd;'>Price</th>
                                        <th style='padding: 12px; text-align: right; border-bottom: 2px solid #ddd;'>Subtotal</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {itemsHtml}
                                </tbody>
                            </table>

                            <!-- Order Summary -->
                            <table style='width: 100%; margin-top: 20px;'>
                                <tr>
                                    <td style='padding: 8px 0; text-align: right; color: #666;'>Subtotal:</td>
                                    <td style='padding: 8px 0 8px 20px; text-align: right; width: 120px; color: #333;'>{FormatPrice(order.SubtotalAmount)}</td>
                                </tr>
                                {(order.DiscountAmount > 0 ? $@"
                                <tr>
                                    <td style='padding: 8px 0; text-align: right; color: #4CAF50;'>Discount:</td>
                                    <td style='padding: 8px 0 8px 20px; text-align: right; color: #4CAF50;'>-{FormatPrice(order.DiscountAmount)}</td>
                                </tr>
                                " : "")}
                                <tr>
                                    <td style='padding: 8px 0; text-align: right; color: #666;'>Shipping:</td>
                                    <td style='padding: 8px 0 8px 20px; text-align: right; color: #4CAF50;'>Free</td>
                                </tr>
                                <tr style='border-top: 2px solid #FFD700;'>
                                    <td style='padding: 15px 0 0 0; text-align: right; font-size: 18px; font-weight: bold; color: #333;'>Total:</td>
                                    <td style='padding: 15px 0 0 20px; text-align: right; font-size: 18px; font-weight: bold; color: #FFD700;'>{FormatPrice(order.TotalAmount)}</td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- Call to Action -->
                    <tr>
                        <td style='padding: 30px; background-color: #f9f9f9; text-align: center;'>
                            <p style='margin: 0 0 20px 0; color: #666; font-size: 14px;'>
                                You can track your order status anytime:
                            </p>
                            <a href='http://localhost:5034/order-history.html' 
                               style='display: inline-block; padding: 15px 40px; background: linear-gradient(135deg, #FFD700 0%, #FFA500 100%); color: #000; text-decoration: none; border-radius: 8px; font-weight: bold; font-size: 16px;'>
                                View Order History
                            </a>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style='padding: 30px; background-color: #333; color: #fff; text-align: center;'>
                            <p style='margin: 0 0 10px 0; font-size: 14px;'>
                                Need help? Contact us at <a href='mailto:support@gymangel.com' style='color: #FFD700; text-decoration: none;'>support@gymangel.com</a>
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

        private static string FormatPrice(decimal price)
        {
            return price.ToString("N0") + " VND";
        }
    }
}
