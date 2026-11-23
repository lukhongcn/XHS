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

        Encoding encoding = string.Equals(barcode.Encoding, "UTF-8", StringComparison.OrdinalIgnoreCase)
            ? Encoding.UTF8
            : Encoding.ASCII;
        return encoding.GetString(data).TrimEnd('\0', ' ');
    }
}
