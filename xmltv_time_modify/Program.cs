using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace xmltv_time_modify
{
  class Program
  {
    static String inputXml = "epg.xml";
    static String outputXml = "epg_times_modified.xml";
    static String configXml = "chans2correct.xml";

    static void Main(string[] args)
    {
      // Get input XMLTV file
      inputXml = (args.Length > 1) ? args[0] : inputXml;
      System.Console.WriteLine("Input EPG file: {0}", inputXml);
      if (!File.Exists(inputXml))
      {
        Console.WriteLine("File {0} not found! Exiting!", inputXml);
        return;
      }

      outputXml = (args.Length > 2) ? args[1] : outputXml;
      Console.WriteLine("Output EPG file: {0}", outputXml);

      // Get configuration XML file
      configXml = (args.Length > 3) ? args[2] : configXml;
      Console.WriteLine("Using config file: {0}", configXml);
      if (!File.Exists(configXml))
      {
        Console.WriteLine("Config XML file {0} not found! Exiting!", inputXml);
        //Console.ReadKey();
        return;
      }
      var chanenlsToCorrect = LoadChannelsToCorrect(configXml);
      Dictionary<String, Double> channelsNotCorrected = chanenlsToCorrect;

      

      XDocument xmlFile = XDocument.Load(inputXml);
      var programmes = from c in xmlFile.Elements("tv").Elements("programme") select c;
      Double offset = 0;
      var channel_id = String.Empty;
      var cOutput = "";

      foreach (XElement programme in programmes)
      {
        var channel_id_temp = programme.Attribute("channel").Value;
        if (chanenlsToCorrect.ContainsKey(channel_id_temp))
        {
          if (channel_id != channel_id_temp) // Is this a new channel?
          {
            if (channel_id != String.Empty)
            {
              Console.Write("\r{0}. Done!              \r\n", cOutput);
              channelsNotCorrected.Remove(channel_id);
            }
            channel_id = channel_id_temp;
            chanenlsToCorrect.TryGetValue(channel_id, out offset);
            var sOffset = offset.ToString("+#;-#;0"); ;
            cOutput = String.Format("Processing channel {0}, offset {1}", channel_id, sOffset);
            Console.Write(cOutput);
          }

          var startTime = programme.Attribute("start").Value;
          TimeCorrect(ref startTime, offset);
          programme.Attribute("start").Value = startTime;

          var endTime = programme.Attribute("stop").Value;
          TimeCorrect(ref endTime, offset);
          programme.Attribute("stop").Value = endTime;
        }
      }
      Console.Write("\r{0}. Done!              \r\n", cOutput);
      xmlFile.Save(outputXml);
      Console.WriteLine("--------------------------------");
      Console.WriteLine("{0} channels were not corrected!", channelsNotCorrected.Count);
      foreach (var k in channelsNotCorrected.Keys)
        Console.WriteLine(k);

      Console.ReadKey();
    }

    static void TimeCorrect(ref String dateTime, Double correction)
    {
      try
      {
        var dateTimeSplit = dateTime.Split(' ');
        DateTime dDateTime = DateTime.ParseExact(dateTimeSplit[0], "yyyyMMddHHmmss", null);
        dDateTime = dDateTime.AddHours(correction);
        dateTime = dDateTime.ToString("yyyyMMddHHmmss") + " " + dateTimeSplit[1];
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error {0}", ex.ToString());
      }
    }
    static Dictionary<String, Double> LoadChannelsToCorrect(String configXml)
    {
      var dict = new Dictionary<String, Double>();
      try
      {
        XmlDocument doc = new XmlDocument();
        doc.Load(configXml);
        XmlNode node = doc.SelectSingleNode("channels");
        foreach (XmlNode childNode in node.ChildNodes)
        {
          if (childNode.Name.Equals("channel"))
          {
            var channelName = childNode.InnerText;
            dict.Add(channelName, Convert.ToDouble(childNode.Attributes["time_error"].Value));
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
      return dict;
    }
  }
}
