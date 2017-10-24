using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace udepilekarsitarafapaketgondermepcapnetolmadan
{
    class GonderaL
    {

        public static Queue kuyruk = new Queue(100);

        public static int gelenPort = 2000;
        public static int gidenPort = 1000;
        public void paketGonder()
        {
            string giris = "";
            while (giris != "ç")
            {
                Console.WriteLine("Gönderilecek mesajı girniz: ");
                giris = Console.ReadLine();
                Byte[] mesaj = Encoding.ASCII.GetBytes(giris);
                using (UdpClient gonderici = new UdpClient(gidenPort))
                    gonderici.Send(mesaj, mesaj.Length, "10.0.2.176", gelenPort);

            }
        }
        public void paketAl()
        {
            UdpClient alici = new UdpClient(gelenPort);
            alici.BeginReceive(AlinanVeri, alici);
        }
        private static void AlinanVeri(IAsyncResult durum)
        {
            UdpClient udpIstemcisi = (UdpClient)durum.AsyncState;
            IPEndPoint Soket = new IPEndPoint(IPAddress.Any, 0);
            Byte[] receivedBytes = udpIstemcisi.EndReceive(durum, ref Soket);

            string alinanmesaj = ASCIIEncoding.ASCII.GetString(receivedBytes);
            lock (kuyruk)
            {
                if (alinanmesaj != "") kuyruk.Enqueue(alinanmesaj);

            }
            udpIstemcisi.BeginReceive(AlinanVeri, durum.AsyncState);

        }
        public void ekranyaz()
        {
            while (true)
            {
                lock (kuyruk)
                {
                    if (kuyruk.Count > 0) Console.WriteLine("Gelen Mesaj:" + kuyruk.Dequeue().ToString());
                }
                Thread.Sleep(1000);

            }
        }
    }
    class Program
    {
        static void Main (string[] args)
        {
            GonderaL yeniGondericiAlici = new GonderaL();
            Thread trGonder = new Thread(new ThreadStart(yeniGondericiAlici.paketGonder));
            Thread trAl = new Thread(new ThreadStart(yeniGondericiAlici.paketAl));
            Thread trYaz = new Thread(new ThreadStart(yeniGondericiAlici.ekranyaz));
            trGonder.Start();
            trAl.Start();
            trYaz.Start();
        }
    }

}

