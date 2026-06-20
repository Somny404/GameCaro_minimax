namespace GAMECARO7X7
{
    public class C_OCo
    {
        public const int CHIEU_RONG = 80;
        public const int CHIEU_CAO = 80;

        public int Dong { get; set; }
        public int Cot { get; set; }
        public int SoHuu { get; set; } // 0: Trống, 1: O, 2: X

        public C_OCo(int dong, int cot, int soHuu)
        {
            Dong = dong;
            Cot = cot;
            SoHuu = soHuu;
        }

        public C_OCo() { }
    }
}