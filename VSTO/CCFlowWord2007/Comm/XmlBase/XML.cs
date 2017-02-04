
public class Row : Hashtable
{
    public Row();
    public Row(Attrs attrs);
    public Row(DataTable dt, DataRow dr);

    public object GetValByKey(string key);
    public void SetValByKey(string key, object val);
}

public abstract class XmlEn
{
    public XmlEn();

    public abstract XmlEns GetNewEntities { get; }
    public Row Row { get; set; }

    public bool GetValBoolByKey(string key);
    public object GetValByKey(string attrKey);
    public decimal GetValDecimalByKey(string key);
    public int GetValIntByKey(string key);
    public string GetValStringByKey(string attrKey);
    public string GetValStringHtmlByKey(string attrKey);
    public int RetrieveBy_del(string key, string val);
    public int RetrieveByPK(string key, string val);
}
public abstract class XmlEnNoName : XmlEn
{
    public XmlEnNoName();
    public XmlEnNoName(string no);

    public string Name { get; }
    public string No { get; }
}