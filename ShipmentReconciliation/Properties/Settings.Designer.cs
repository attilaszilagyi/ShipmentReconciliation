﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShipmentReconciliation.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.9.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool GenerateData {
            get {
                return ((bool)(this["GenerateData"]));
            }
            set {
                this["GenerateData"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ProcessData {
            get {
                return ((bool)(this["ProcessData"]));
            }
            set {
                this["ProcessData"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoStart {
            get {
                return ((bool)(this["AutoStart"]));
            }
            set {
                this["AutoStart"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoExit {
            get {
                return ((bool)(this["AutoExit"]));
            }
            set {
                this["AutoExit"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Verbose {
            get {
                return ((bool)(this["Verbose"]));
            }
            set {
                this["Verbose"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DATA\\TEST")]
        public string FolderPath {
            get {
                return ((string)(this["FolderPath"]));
            }
            set {
                this["FolderPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool FolderSearchSubs {
            get {
                return ((bool)(this["FolderSearchSubs"]));
            }
            set {
                this["FolderSearchSubs"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("*CustomerOrders*.csv")]
        public string FolderSearchPatternCustomerOrders {
            get {
                return ((string)(this["FolderSearchPatternCustomerOrders"]));
            }
            set {
                this["FolderSearchPatternCustomerOrders"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("*FactoryShipment*.csv")]
        public string FolderSearchPatternFactoryShipment {
            get {
                return ((string)(this["FolderSearchPatternFactoryShipment"]));
            }
            set {
                this["FolderSearchPatternFactoryShipment"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string FilePathCustomerOrders {
            get {
                return ((string)(this["FilePathCustomerOrders"]));
            }
            set {
                this["FilePathCustomerOrders"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string FilePathFactoryShipment {
            get {
                return ((string)(this["FilePathFactoryShipment"]));
            }
            set {
                this["FilePathFactoryShipment"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3000")]
        public int GenerateDataMaxNumberOfProducts {
            get {
                return ((int)(this["GenerateDataMaxNumberOfProducts"]));
            }
            set {
                this["GenerateDataMaxNumberOfProducts"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("150000")]
        public int GenerateDataMaxNumberOfOrders {
            get {
                return ((int)(this["GenerateDataMaxNumberOfOrders"]));
            }
            set {
                this["GenerateDataMaxNumberOfOrders"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("4000")]
        public int GenerateDataMaxNumberOfCustomers {
            get {
                return ((int)(this["GenerateDataMaxNumberOfCustomers"]));
            }
            set {
                this["GenerateDataMaxNumberOfCustomers"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3000")]
        public int GenerateDataMaxQuantityPerOrder {
            get {
                return ((int)(this["GenerateDataMaxQuantityPerOrder"]));
            }
            set {
                this["GenerateDataMaxQuantityPerOrder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10000")]
        public int GenerateDataMaxQuantityPerProduct {
            get {
                return ((int)(this["GenerateDataMaxQuantityPerProduct"]));
            }
            set {
                this["GenerateDataMaxQuantityPerProduct"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(";")]
        public string CsvConfigurationDelimiter {
            get {
                return ((string)(this["CsvConfigurationDelimiter"]));
            }
            set {
                this["CsvConfigurationDelimiter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("UTF-8")]
        public string CsvConfigurationEncoding {
            get {
                return ((string)(this["CsvConfigurationEncoding"]));
            }
            set {
                this["CsvConfigurationEncoding"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("en-GB")]
        public string CsvConfigurationCulture {
            get {
                return ((string)(this["CsvConfigurationCulture"]));
            }
            set {
                this["CsvConfigurationCulture"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Fulfill.csv")]
        public string ResultFileNameFulfill {
            get {
                return ((string)(this["ResultFileNameFulfill"]));
            }
            set {
                this["ResultFileNameFulfill"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ResultFolderPath {
            get {
                return ((string)(this["ResultFolderPath"]));
            }
            set {
                this["ResultFolderPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Store.csv")]
        public string ResultFileNameStore {
            get {
                return ((string)(this["ResultFileNameStore"]));
            }
            set {
                this["ResultFileNameStore"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool DisplayData {
            get {
                return ((bool)(this["DisplayData"]));
            }
            set {
                this["DisplayData"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool DisplayResult {
            get {
                return ((bool)(this["DisplayResult"]));
            }
            set {
                this["DisplayResult"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000")]
        public int OptimizerLimit {
            get {
                return ((int)(this["OptimizerLimit"]));
            }
            set {
                this["OptimizerLimit"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool DisplaySettings {
            get {
                return ((bool)(this["DisplaySettings"]));
            }
            set {
                this["DisplaySettings"] = value;
            }
        }
    }
}
