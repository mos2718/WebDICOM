using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace DrawDICOMObject
{  
    public partial class Form1 : Form
    {
        string DICOMTagXMLFile;
        byte[] DICOMObject = null;
        short[] PixelArray = null;
        int  Rows;    //00280010
        int  Columns; // 00280011
  
/*        string PhotometricInterpretation = "MONOCHROME1"; //00280004 exp: MONOCHROME1
        int BitsAllocated = 16;  //00280100  exp: 16 or 8
        int PixelRepresentation =1;  //00280103  exp: 1 signed data   0:unsigned  */
        int WindowCenter = 40;
        int WindowWidth =400;
     

        int Offset;
        int Length;

       // DICOMTagExtractor.DICOMToXMLInterface MyDICOMToXML = new DICOMTagExtractor.DICOMToXMLInterface();
             
        public Form1()
        {
            InitializeComponent();

            
        }

        void GetXMLAttributes(string  SourceFile)
        {
            XmlDocument m_Doc;
            XmlNode TagNode;
            XmlNodeList nodelist;
            m_Doc = new XmlDocument();

            m_Doc.Load(SourceFile);  // load XML file
            nodelist = m_Doc.GetElementsByTagName("Tag00280010");
            TagNode = nodelist[0]; 
            textBox1.Text = TagNode.FirstChild.Value;

            nodelist = m_Doc.GetElementsByTagName("Tag00280011");
            TagNode = nodelist[0];
            textBox2.Text = TagNode.FirstChild.Value;  
            
            nodelist = m_Doc.GetElementsByTagName("Tag7fe00010");
            TagNode = nodelist[0];
            XmlAttributeCollection atts = TagNode.Attributes;
            XmlAttribute att = atts["Offset"];  //offset attribute
            textBox3.Text = att.Value;


        }

        private void button1_Click(object sender, EventArgs e)
        {

         if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {   
                label1.Text = openFileDialog1.FileName;
                int FileLength; 
                FileLength = label1.Text.Length;
                string DICOMTagXMLFile;
                DICOMTagXMLFile = label1.Text.Substring(0, FileLength - 4);
                DICOMTagXMLFile = DICOMTagXMLFile + ".xml";
                label1.Text = DICOMTagXMLFile;
                GetXMLAttributes( DICOMTagXMLFile);        
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {


            Rows = Convert.ToInt32(textBox1.Text);
            Columns = Convert.ToInt32(textBox2.Text);
            Offset = Convert.ToInt32(textBox3.Text);
            WindowCenter = Convert.ToInt32(textBox4.Text);
            WindowWidth = Convert.ToInt32(textBox5.Text);
            PixelArray = new short[Rows * Columns];

            DICOMObject = File.ReadAllBytes(openFileDialog1.FileName);


            int i, j, k;

            // For 16 bit image 
           
            for (i = 0; i < Rows; i++)
                for (j = 0; j < Columns; j++)
                {
                    k = i * Columns + j;
                    PixelArray[k] = System.BitConverter.ToInt16(DICOMObject, Offset + 2 * k);

                }
            // For 8 bit image 
            /*
            for (i = 0; i < Rows; i++)
                for (j = 0; j < Columns; j++)
                {
                    k = i * Columns + j;
                    PixelArray[k] = Convert.ToInt16(System.BitConverter.ToChar(DICOMObject, Offset + k));

                }
            */

                Bitmap result = new Bitmap( Columns,Rows);
                
                int  Max, Min, GrayValue;
                bool MONOCHROME1 =false;

                if(checkBox1.Checked == true)
                    MONOCHROME1 = true;

                Max = WindowCenter + WindowWidth / 2;
                Min = WindowCenter - WindowWidth / 2;
                for (i = 0; i < Rows; i++)
                {
                    for (j = 0; j < Columns; j++)
                    {
                        k = i * Columns + j;
                        GrayValue = PixelArray[k];
                        GrayValue = (PixelArray[k] - Min) * 256 / WindowWidth;
                        if (GrayValue >= 256) GrayValue = 255;
                        if (GrayValue < 0) GrayValue = 0;
                        if (MONOCHROME1 == true)             GrayValue = 255 - GrayValue;
                        result.SetPixel(j, i, Color.FromArgb(GrayValue, GrayValue, GrayValue));
                    }
                }
                pictureBox1.Image = result;   //把結果顯示到PictureBox中
                
        }

  
    }
}
