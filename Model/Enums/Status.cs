using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Enums
{
    public enum BookingStatus
    {
        newBooking,
        canceled,
        confirmed,
        completed,
    }
    public static class OrderEnums
    {
        public static class FloorTableStatus
        {
            public static string booked { get { return nameof(booked); } }
            public static string available { get { return nameof(available); } }

        }
        public static class Status
        {
            public static string cooking { get { return nameof(cooking); } }
            public static string hide { get { return nameof(hide); } }
            public static string complete { get { return nameof(complete); } }
            public static string delivered { get { return nameof(delivered); } }
            public static string cancel { get { return nameof(cancel); } }
            public static string ready { get { return nameof(ready); } }
        }
        public static class PaymentStatus
        {
            public static string pending { get { return nameof(pending); } }
            public static string completed { get { return nameof(completed); } }
            public static string refunded { get { return nameof(refunded); } }
        }

        public static class DeliveryType
        {
            public static string delivery { get { return nameof(delivery); } }
            public static string collected { get { return nameof(collected); } }
            public static string waiting { get { return nameof(waiting); } }
            public static string eatin { get { return nameof(eatin); } }
        }
        public static class EatInPaymentStatus
        {
            public static string pending { get { return nameof(pending); } }
            public static string completed { get { return nameof(completed); } }
        }
    }
}
