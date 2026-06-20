using System.Drawing;

namespace GAMECARO7X7
{
    public class C_BanCo
    {
        private static Image ImageO = new Bitmap(Properties.Resources.O);
        private static Image ImageX = new Bitmap(Properties.Resources.X);

        public int SoDong { get; set; }
        public int SoCot { get; set; }

        public C_BanCo(int soDong, int soCot)
        {
            SoDong = soDong;
            SoCot = soCot;
        }

        public void VeBanCo(Graphics g)
        {
            for (int i = 0; i <= SoCot; i++)
                g.DrawLine(C_DieuKhien.pen, i * C_OCo.CHIEU_RONG, 0, i * C_OCo.CHIEU_RONG, SoDong * C_OCo.CHIEU_CAO);

            for (int i = 0; i <= SoDong; i++)
                g.DrawLine(C_DieuKhien.pen, 0, i * C_OCo.CHIEU_CAO, SoCot * C_OCo.CHIEU_RONG, i * C_OCo.CHIEU_CAO);
        }

        public void VeQuanCo(Graphics g, int x, int y, int soHuu)
        {
            if (soHuu == 1)
                g.DrawImage(ImageO, x, y, C_OCo.CHIEU_RONG, C_OCo.CHIEU_CAO);
            else if (soHuu == 2)
                g.DrawImage(ImageX, x + 2, y + 2, C_OCo.CHIEU_RONG - 4, C_OCo.CHIEU_CAO - 4);
        }
    }
}