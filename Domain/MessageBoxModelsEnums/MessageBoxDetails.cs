using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MessageBoxModelsEnums
{
	public class MessageBoxDetails
	{
		public MessageBoxType MessageType { get; set; }

		public MessageBoxButtonType MessageButtons { get; set; }

		public MessageBoxIconType MessageIcon { get; set; }

		public MessageBoxBorderType MessageBorder { get; set; }

		public string Title { get; set; }

		public string Content { get; set; }

		public List<string> Choices { get; set; }
	}
}