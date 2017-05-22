﻿using System;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;
using Mediachase.Commerce.Catalog;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using MetaClass = Mediachase.MetaDataPlus.Configurator.MetaClass;
using MetaField = Mediachase.MetaDataPlus.Configurator.MetaField;

namespace Klarna.Checkout.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Commerce.Initialization.InitializationModule))]
    internal class MetadataInitialization : IInitializableModule
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(MetadataInitialization));

        public void Initialize(InitializationEngine context)
        {
            MetaDataContext mdContext = CatalogContext.MetaDataContext;
            
            JoinField(mdContext, GetOrCreateCardField(mdContext, Constants.KlarnaCheckoutOrderIdField), Common.Constants.PurchaseOrderClass);
            JoinField(mdContext, GetOrCreateCardField(mdContext, Common.Constants.KlarnaSerializedMarketOptions), Common.Constants.OtherPaymentClass);
        }

        public void Uninitialize(InitializationEngine context)
        {

        }

        private MetaField GetOrCreateCardField(MetaDataContext mdContext, string fieldName, MetaDataType metaDataType = MetaDataType.LongString)
        {

            var f = MetaField.Load(mdContext, fieldName);
            if (f == null)
            {
                Logger.Debug($"Adding meta field '{fieldName}' for Klarna checkout integration.");
                f = MetaField.Create(mdContext, Common.Constants.OrderNamespace, fieldName, fieldName, string.Empty, metaDataType, Int32.MaxValue, true, false, false, false);
            }
            return f;
        }
        
        private void JoinField(MetaDataContext mdContext, MetaField field, string metaClassName)
        {
            var cls = MetaClass.Load(mdContext, metaClassName);

            if (MetaFieldIsNotConnected(field, cls))
            {
                cls.AddField(field);
            }
        }
        
        private static bool MetaFieldIsNotConnected(MetaField field, MetaClass cls)
        {
            return cls != null && !cls.MetaFields.Contains(field);
        }
    }
}
