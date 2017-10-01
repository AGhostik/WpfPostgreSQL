namespace WpfPostgreSQL.Model
{
    public enum ChipherAlgo
    {
        NoCrypt,
        Blowfish,
        AES_128,
        PGP_Blowfish,
        PGP_AES_128,
        PGP_AES_192,
        PGP_AES_256,
        PGP_ThripleDes,
        PGP_Cast5
    }
}
