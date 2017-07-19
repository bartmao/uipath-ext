using System;
using System.Activities.Presentation.Metadata;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivitiesEx
{
    public class MyRegister : IRegisterMetadata
    {
        public void Register()
        {
            var attributeTableBuilder = new AttributeTableBuilder();
            // Category attribute not working in UIPath, not known yet
            attributeTableBuilder.AddCustomAttributes(typeof(InvokeMethodEx), new Attribute[]
            {
                new CategoryAttribute("Bart.Infrastructure")
            });
            MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
        }
    }
}
