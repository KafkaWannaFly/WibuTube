namespace WibuTube
{
    public interface ISong
    {
        public string Tittle { get; set; }
        public string[] Performers { get; set; }
        public string Album { get; set; }
    }
}