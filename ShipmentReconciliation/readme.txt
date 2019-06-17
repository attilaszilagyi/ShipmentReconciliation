ShipmentReconciliation.exe

  Windows desktop console application to reconciliate Customer Order records with Factory Shipment records.
  
Operations:

	* GenerateData: automatically generates random test input data and saves into file system
	* ProcessData: reconciliates the loaded or generated input data records and displays and/or saves the result to the file system.

Remarks:
	The reconciler logic uses optimization with a simple algorithm and a 01-KnapSack problem solver to minimize product surplus to store.

Main Steps:
    1., Generates input data and stores to csv files or 
    2., Loads input data from csv files and 
    3., Processes input data, reconciliating shipments with orders 
    (optimization with simple greedy algorithm and 01-knapsack problem solver minimizes surplus to store) and
    4., Lists result on screen and/or saves them to file.

Application Settings:

  DATATYPE  NAME                                    DEFAULT                         NOTES
  
  bool      AutoExit:                               False                           Set to "True" to automatically exit application when finished performing the selected operation(s), otherwise the program will asks for user confirmation.
  bool      AutoStart:                              False                           Set to "True" to automatically start performing the selected operation(s), otherwise the program will asks for user confirmation.
  string    CsvConfigurationCulture:                en-GB                           Csv file option. Language code table to use.
  string    CsvConfigurationDelimiter:              ;                               Csv file option. Field separator character.
  string    CsvConfigurationEncoding:               UTF-8                           Csv file option. File character encoding.
  bool      DisplayData:                            True                            Lists all customer order and factory shipment input data records. Works only if Verbose=True too.
  bool      DisplayResult:                          True                            Lists all result records: customer orders to fulfill and product quantities to store. Works only if Verbose=True too.
  bool      DisplaySettings:                        True                            Lists all setting values before starting any operation. Works only if Verbose=True too.
  string    FilePathCustomerOrders:                                                 Absolute or relative path of the customer orders csv file. Works in pair with FilePathFactoryShipment. Otherwise falls back to FolderPath.
  string    FilePathFactoryShipment:                                                Absolute or relative path of the factory shipment csv file. Works in pair with FilePathCustomerOrders. Otherwise falls back to FolderPath.
  string    FolderPath:                             DATA\TEST                       Absolute or relative path. If no file paths provided, then the program searches for this folder (and its subfolders if FolderSearchSub=true) with the provided search patterns and loads all files found. This value is also used for saving result files in case no ResultFolderPath provided.
  string    FolderSearchPatternCustomerOrders:      *CustomerOrders*.csv            Works in pair with FolderPath.
  string    FolderSearchPatternFactoryShipment:     *FactoryShipment*.csv           Works in pair with FolderPath.
  bool      FolderSearchSubs:                       False                           Works in pair with FolderPath. True: AllSubDirectories. False: TopDirectoryOnly.
  bool      GenerateData:                           False                           One of the two main operation directives (the other one is: ProcessData). Set to "True" to automatically generate random input data values (eg. for test purposes).
  int       GenerateDataMaxNumberOfCustomers:       4 000                           Test input data generator parameter.
  int       GenerateDataMaxNumberOfOrders:          150 000                         Test input data generator parameter.
  int       GenerateDataMaxNumberOfProducts:        3 000                           Test input data generator parameter.
  int       GenerateDataMaxQuantityPerOrder:        3 000                           Test input data generator parameter.
  int       GenerateDataMaxQuantityPerProduct:      10 000                          Test input data generator parameter.
  int       OptimizerLimit:                         1 000                           Maximum number of different combinations to try by the 01-KnapSack problem solver. Zero: no limit.
  bool      ProcessData:                            True                            The main directive to enable reconciliation operation (the other one is: GenerateData).
  string    ResultFileNameFulfill:                  Fulfill.csv                     Provide an empty string if you don't want to save the reconciliation result records to file.
  string    ResultFileNameStore:                    Store.csv                       Provide an empty string if you don't want to save the reconciliation result records to file.
  string    ResultFolderPath:                                                       If empty, it falls back to FolderPath.
  bool      Verbose:                                True                            Set it to "False" if You don't want to see detailed progress reports, operation phase summaries, and detailed tables/lists of records.


The default values can be changed in the ShipmentReconciliation.exe.config file

Command line parameters:

  Every application setting can be provided as command line parameter in the form: Name=Value (eg. OptimizerLimit=100).
  In case of bool variables you may omit "=true" (eg. AutoExit=True equals AutoExit). 
  Values containing white space caracters must be enclosed in double quotes (eg. FolderPath="DATA\Test folder 001").

  Examples:
	
	ShipmentReconciliation.exe GenerateData=true ProcessData=true AutoStart Verbose=true FolderPath= ResultFolderPath= DisplaySettings=false DisplayData=false DisplayResult=false
	ShipmentReconciliation.exe GenerateData=true ProcessData=false AutoStart Verbose=false FolderPath=DATA\AUTO DisplayData DisplayResult
	ShipmentReconciliation.exe GenerateData=false ProcessData=true AutoStart Verbose=true FolderPath=DATA\TEST FolderSearchSubs ResultFolderPath= DisplaySettings=false DisplayData=false DisplayResult=false
	ShipmentReconciliation.exe GenerateData=true ProcessData=true AutoStart Verbose=false FolderPath=DATA\AUTO DisplaySettings=false DisplayData=false DisplayResult=false


Default Files:
	* CustomerOrders.csv             Input file. Customer Order records to reconciliate. Fields: Order ID, Customer ID, Item Name, Quantity.
	* FactoryShipments.csv           Input file. Factory Shipment records to reconciliate. Fields: Item Name, Quantity.
	* Fulfill.csv                    Output file. Customer Order records to fulfill. Fields: OrderID, CustomerID, ItemName, Quantity.
	* Store.csv                      Output file. Product items (surplus quantites) to store. Fields: ItemName, Quantity.


Author:
	Attila Szilagyi	
	http://szilagyiattila.hu
	email: contact@szilagyiattila.hu
	phone: +36205716499

Credits/Nuget packages: 
	Microsoft.Solver.Foundation
	CsvHelper


Copyright@Attila Szilagyi, 2019