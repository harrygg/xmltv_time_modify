# xmltv_time_modify
A simple program to modify start and end times in XMLTV guide. Does the same thing as the xmltv_time_correct from WebGrab++ website and uses its configuration files.
If a programme for a channel is not found an error message is logged in the console and the program continues with the next channel. 
Unlike the xmltv_time_correct which halts and waits for the user to press any key (not good for automatcion).

# Usage:
.\xmltv_time_modify.exe [input_epg_file] [output_epg_file] [custom_configuration_file]

# Sample configration file:
<channels>
  <!--
  This file specifies the channels to be 'time' corrected by xmltv_time_correct.exe
  Syntax of this file:
  <channel time_error="+1">channel-name</channel>
  -  time_error : the number of (decimal)hours the channel is 'off',
   (so, if you want all shows of a channel 1 hour later in the output xmltv file, specify time_error="-1",
   for 1:30 minutes earlier, specify time_error="1.5")
  - channel-name, the xmltv_id as in webgrab++.config.xml of the channel you want to correct
  Examples:
  -->
  <channel time_error="+2">Sky Sports 1</channel>
</channels>
