using System;
using System.Text;

/// <summary>
/// MES 业务辅助类。这里只处理请求位和条码内容，不负责 PLC 连接。
/// </summary>
public sealed class MESHelp
{
    public bool IsBarcodeRequest(byte value, PlcBitConfig interaction)
    {
        if (interaction == null) throw new ArgumentNullException(nameof(interaction));
        return (value & (1 << interaction.Bit)) != 0;
    }

    public byte SetBarcodeRequest(byte value, PlcBitConfig interaction)
    {
        if (interaction == null) throw new ArgumentNullException(nameof(interaction));
        return (byte)(value | (1 << interaction.Bit));
    }

    public byte ClearBarcodeRequest(byte value, PlcBitConfig interaction)
    {
        if (interaction == null) throw new ArgumentNullException(nameof(interaction));
        return (byte)(value & ~(1 << interaction.Bit));
    }

    public string DecodeBarcode(byte[] data, BarcodeConfig barcode)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (barcode == null) throw new ArgumentNullException(nameof(barcode));
        if (data.Length < 2) return string.Empty;

        // Siemens STRING uses the first byte for the maximum length and the
        // second byte for the current length. The actual text starts at +2.
        int maxLength = data[0];
        int actualLength = data[1];
        if (maxLength <= 0 || actualLength <= 0) return string.Empty;
        int length = Math.Min(actualLength, maxLength);
        length = Math.Min(length, data.Length - 2);

        Encoding encoding = string.Equals(barcode.Encoding, "UTF-8", StringComparison.OrdinalIgnoreCase)
            ? Encoding.UTF8
            : Encoding.ASCII;
        return encoding.GetString(data, 2, length).TrimEnd('\0', ' ');
    }
}
