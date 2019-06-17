namespace ShipmentReconciliation
{
  public static class StringEx
  {

    public static string TrimPath(this string text, int max = 50)
    {
      return (text.Length > max) ? "..." + text.Substring(text.Length - max) : text;
    }

  }
}
