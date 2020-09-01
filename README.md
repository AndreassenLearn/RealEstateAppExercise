# Opgave 1.3 - RealEstateApp projektet, med UI men metoderne er tomme
Denne opgave g�r ud p� at lave en ejendomsm�gler app kaldet RealEstateApp. Den laves i Xamarin.Forms og har to form�l: Tr�ning i grundl�ggende app-programmering samt at have et projekt, hvor det giver god mening at indbygge forskellige sensorer.  App'en har f�lgende sk�rmbilleder:

| ![Properties List](images/PropertiesList.png) PropertyListPage | ![Detail View](images/DetailView.png) PropertyDetailPage | ![Edit View](images/EditView.png) AddEditPage (EDIT) |  
|---|---|---|
|---|---|---|
| ![Add](images/Add.png) AddEditPage (ADD) | ![Flyout Menu](images/FlyoutMenu.png) MenuPage | ![About](images/About.png) AboutPage |  
|   |   |   |  
|   |   |   | 



I denne branch, som hedder 1.3.UInoCodeBehind, er al XAML-kode p� plads og diverse eventhandlers og metoder er oprettet - men er uden indhold.

Hvis du ikke allerede har gjort det, s� begynd med at hente de forrige to branches ned fra GitHub og gennemg� dem grundigt. V�r sikker p� at du forst�r alt inden du bygger videre p� dem.



### PropertyListPage
PropertyListPage er allerede oprettet. PropertyListPage.xaml.cs indeholder en property kaldet PropertiesCollection, som indeholder objekter af typen PropertyListItem og som der kan bindes til n�r alle husene skal vises. Constructoren f�r fat i et objekt af MockRepository ved at lave  Resolve af IRepository. Derefter hentes alle Property-objekterne vha. GetProperties() og BindingContext kan s�tte op. Start med at f� det til at lykkedes, inden du g�r videre med xaml-koden.



### PropertyDetailPage

 Dens constructor skal kunne tage imod et Property objekt, som der bliver valgt i PropertyListPage. F� parameteroverf�relsen til at virke, inden du g�r videre.

N�r du har Property objektet, kan du ogs� hente det aktuelle AgentId ved at sl� op i Repository. Nu b�r du have to properties, Property og Agent, som du kan binde til View'et.



### AddEditPropertyPage
Denne page kan man komme til b�de fra Add | PropertyListPage og fra Edit | PropertyDetailPage. Kommer vi fra Edit, skal der kunne tages imod et Property objekt i constructoren. Om der er en parameter med eller ej kan benyttes til at styre Title, som enten skal v�re "Add Property" eller "Edit Property".

Man kan ogs� lave Save-eventhandleren og i f�rste omgang f� navigationen til at g� tilbage til rod-siden.

Nu kan View'et laves med binding til Property og Agent.

Hvis du har overskud til det, b�r der laves noget validering af brugerinput.
