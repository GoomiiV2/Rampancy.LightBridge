using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Formatting.Display;

namespace Rampancy.LightBridge
{
    public class TextBoxLogSink : ILogEventSink
    {
        private readonly MessageTemplateTextFormatter FormatProvider;
        public RichTextBox LogTB;

        public TextBoxLogSink(string template, RichTextBox logTB)
        {
            FormatProvider = new MessageTemplateTextFormatter(template ?? "[{Level:u3}] {Message:lj}{NewLine}{Exception}");
            LogTB           = logTB;
        }

        public void Emit(LogEvent logEvent)
        {
            using var writer = new StringWriter();
            FormatProvider.Format(logEvent, writer);
            var message = writer.ToString();
            LogTB.Invoke(() =>
            {
                LogTB.AppendText(message);
            });
        }
    }

    public static class LogSinkExtensions
    {
        public static LoggerConfiguration TextBoxLogSink(this LoggerSinkConfiguration logConfig, RichTextBox logTB, string template = null)
        {
            return logConfig.Sink(new TextBoxLogSink(template, logTB));
        }
    }
}
