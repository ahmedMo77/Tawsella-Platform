using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Domain.Enums
{
    public enum NotificationType
    {
        OrderCreated,
        OrderAccepted,
        OrderPickedUp,
        OrderDelivered,
        OrderCancelled,
        PaymentReceived,
        AccountApproved,
        AccountRejected,
        ApplicationApproved,
        ApplicationRejected,
        WithdrawalProcessed,
        General
    }
}
