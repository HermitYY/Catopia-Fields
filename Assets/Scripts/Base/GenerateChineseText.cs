using System.Text;
using System.IO;
using UnityEngine;

public class GenerateChineseText : MonoBehaviour
{
    void Start()
    {
        // 1. 创建 StringBuilder
        StringBuilder sb = new StringBuilder();

        // 2. 循环添加一级汉字 (Unicode 0x4E00-0x9FA5)
        for (int i = 0x4E00; i <= 0x9FA5; i++)
        {
            sb.Append((char)i);
        }

        // 3. 保存到 txt 文件
        string path = Application.dataPath + "/ChineseCharacters.txt"; // 保存到 Assets 根目录
        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);

        Debug.Log("汉字文本已生成到: " + path);
    }
}
