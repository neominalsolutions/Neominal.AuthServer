Font Channel: Authorize Endpointe yapılan istekleri temsil eder.
Back Channel: Token Endpointe yapılan istek kanalı


Grant Types

Authorization Code Grant: Üyelik sistemi olan altyapılar için tercih edilir. 2 aşamalı doğrulamadan oluşur. 1. aşamda Authorize Endpointe gidilerek Authorization Code alanır. Daha sonra bu Authorization Code ile Token Endpointe gidilerek Access Token alınır. Farklı firmaların client uygulamalarını kendi identity serverımız üzerinden kendi ApiResources'larımıza eriştirmek için üyelik sistemi kullanıyorsak tercih ettiğimizi bir altyapıdır. Net Core MVC gibi server-side taraflı çözümlerde tercih edilebilir.

Implicit Grant: Üyelik sistemi olan altyapılar için tercih edilir. Tek aşamalı doğrulamadan oluşur. Browserdan direkt olarak Authorize endpointe istek atılır ve Acess Token alınır. SPA ve Mobile uygulamalar için tercih edilen bir çözümdür. 

Resource Owner Credential Grant: Üyelik sistemi olan altyapılarda tercih edilir. Eğer şirket içi client-side ve server-side taraflı uygulamalar geliştiriyorsak, sadece client ve server taraflı çözümler bizim üzerimizde yaygınlanıyorsa tercih edilen yöntemdir. Bu durumda direk olarak Token Endpointe istek atılarak.Access Token alınabilir. Implicit Grant dan farkı ise birisinin token FrontChanneldan alması Resource Owner Credential Grant ise BackChanneldan almasıdır.

Client Credentials : Üyelik sistemi olmayan alt yapılarda tercih edilir. Client_Id ve Client_Secret ile sunucu ve istemci birbirleri ile güven dahilinde haberleşebilir. 

Authorize Endpointine istek attığımızda belirli return typelar verebiliriz. Bu return typelar code token code id_token cinsinden olabilir. Bu return typeları client tarafında verebiliyoruz. Code Authorize Code karşılık gelirken token ise Acess Token  karşılık gelir. Server side teknolojiler için Authorize Endpointinden access token direkt olarak dönülmesi doğru değildir. id_Token ise identity token anlamına gelir. Id Token İdentity Token kullanıcı sisteme girdikten sonra kullanıcıya ait kimlik bilgilerinin doğrulanmasını sağlayan JETON. Authrize EndPointten code, id_token ve token da istenebilir.Birden fazla token alma biçimine hibrid akış Hybrid Flow denir.En uygun olan server-side uygulamalar için code id_token ikilisidir.Id_Token yine bir Json Web Token fakat, içerisinde sadece oturum açan kullanıcı idsi barındırır.client tarafından doğru bir authorize code gelip gelmediğini anlamak için kullanılır. 
