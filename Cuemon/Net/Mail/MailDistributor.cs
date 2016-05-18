using System;
using System.Collections.Generic;
using System.Net.Mail;
using Cuemon.Collections.Generic;
using Cuemon.Threading;

namespace Cuemon.Net.Mail
{
    /// <summary>
    /// Provides a way for applications to distribute one or more e-mails in batches by using the Simple Mail Transfer Protocol (SMTP).
    /// </summary>
    public class MailDistributor
    {
        private static readonly bool IsSmtpClientDisposable = new Doer<bool>(() => TypeUtility.ContainsInterface(typeof(SmtpClient), typeof(IDisposable)))(); // CLR2 does not implement the IDisposable pattern, but CLR4 does.

        /// <summary>
        /// Initializes a new instance of the <see cref="MailDistributor"/> class.
        /// </summary>
        /// <param name="carrier">The function delegate that will instantiate a new mail carrier per delivery.</param>
        public MailDistributor(Doer<SmtpClient> carrier) : this(carrier, 20)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MailDistributor" /> class.
        /// </summary>
        /// <param name="carrier">The function delegate that will instantiate a new mail carrier per delivery.</param>
        /// <param name="deliverySize">The maximum number of mails a <paramref name="carrier"/> can deliver at a time. Default is a size of 20.</param>
        /// <remarks>
        /// A delivery is determined by the <paramref name="deliverySize"/>. This means, that if you are to send 100 e-mails and you have a <paramref name="deliverySize"/> of 20, 
        /// these 100 e-mails will be distributed to 5 invoked instances of <paramref name="carrier"/> shipping up till 20 e-mails each (depending if you have a filter or not).
        /// </remarks>
        public MailDistributor(Doer<SmtpClient> carrier, int deliverySize)
        {
            Validator.ThrowIfNull(carrier, nameof(carrier));
            Validator.ThrowIfLowerThan(deliverySize, 1, nameof(deliverySize));
            Carrier = carrier;
            DeliverySize = deliverySize;
        }

        private Doer<SmtpClient> Carrier { get; }

        private int DeliverySize { get; }

        /// <summary>
        /// Sends the specified <paramref name="mail"/> to an SMTP server.
        /// </summary>
        /// <param name="mail">The e-mail to send to an SMTP server.</param>
        public void SendOne(MailMessage mail)
        {
            SendOne(mail, null);
        }

        /// <summary>
        /// Sends the specified <paramref name="mail"/> to an SMTP server.
        /// </summary>
        /// <param name="mail">The e-mail to send to an SMTP server.</param>
        /// <param name="filter">The function delegate that defines the conditions for the sending of <paramref name="mail"/>.</param>
        /// <remarks>The function delegate <paramref name="filter"/> will only include the <paramref name="mail"/> if that evaluates to <c>true</c>.</remarks>
        public void SendOne(MailMessage mail, Doer<MailMessage, bool> filter)
        {
            Validator.ThrowIfNull(mail, nameof(mail));
            Send(EnumerableUtility.Yield(mail), filter);
        }

        /// <summary>
        /// Sends the specified sequence of <paramref name="mails"/> to an SMTP server.
        /// </summary>
        /// <param name="mails">The e-mails to send to an SMTP server.</param>
        public void Send(IEnumerable<MailMessage> mails)
        {
            Send(mails, null);
        }

        /// <summary>
        /// Sends the specified sequence of <paramref name="mails"/> to an SMTP server.
        /// </summary>
        /// <param name="mails">The e-mails to send to an SMTP server.</param>
        /// <param name="filter">The function delegate that defines the conditions for sending of the <paramref name="mails"/> sequence.</param>
        /// <remarks>The function delegate <paramref name="filter"/> will only include those <paramref name="mails"/> that evaluates to <c>true</c>.</remarks>
        public void Send(IEnumerable<MailMessage> mails, Doer<MailMessage, bool> filter)
        {
            Validator.ThrowIfNull(mails, nameof(mails));
            var carriers = PrepareShipment(this, mails, filter);
            var shipments = new List<ThreadPoolTask>();
            foreach (var shipment in carriers)
            {
                var filteredMails = shipment.Arg2;
                if (filteredMails.Count == 0) { continue; }
                var carrier = shipment.Arg1;
                shipments.Add(ThreadPoolUtility.RunAction(Send, carrier, filteredMails));
            }
            ThreadPoolUtility.WaitAll(shipments);
        }

        private static void Send(Doer<SmtpClient> carrierCallback, List<MailMessage> mails)
        {
            var carrier = carrierCallback();
            try
            {
                if (!IsSmtpClientDisposable) { AdjustCarrierToClr2Compatibility(carrier, mails.Count); }
                foreach (var mail in mails)
                {
                    try
                    {
                        carrier.Send(mail);
                    }
                    finally
                    {
                        if (mail != null) { mail.Dispose(); }
                    }
                }
            }
            finally
            {
                if (carrier != null && IsSmtpClientDisposable) // in case of CLR 4.0
                {
                    ((IDisposable)carrier).Dispose();
                }
            }
        }

        private static List<Template<Doer<SmtpClient>, List<MailMessage>>> PrepareShipment(MailDistributor distributor, IEnumerable<MailMessage> mails, Doer<MailMessage, bool> filter)
        {
            PartitionCollection<MailMessage> partitionedMails = new PartitionCollection<MailMessage>(mails, distributor.DeliverySize);
            List<Template<Doer<SmtpClient>, List<MailMessage>>> carriers = new List<Template<Doer<SmtpClient>, List<MailMessage>>>();
            while (partitionedMails.HasPartitions)
            {
                carriers.Add(TupleUtility.CreateTwo(distributor.Carrier, new List<MailMessage>(filter == null
                    ? partitionedMails
                    : EnumerableUtility.FindAll(partitionedMails, filter))));
            }
            return carriers;
        }

        private static void AdjustCarrierToClr2Compatibility(SmtpClient carrier, int connections)
        {
            carrier.ServicePoint.MaxIdleTime = (int)(TimeSpan.FromSeconds(1).TotalMilliseconds * connections);
            carrier.ServicePoint.ConnectionLimit = connections;
        }
    }
}