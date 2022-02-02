Font Channel: Authorize Endpointe yapılan istekleri temsil eder.
Back Channel: Token Endpointe yapılan istek kanalı


Grant Types

Authorization Code Grant: Üyelik sistemi olan altyapılar için tercih edilir. 2 aşamalı doğrulamadan oluşur. 1. aşamda Authorize Endpointe gidilerek Authorization Code alanır. Daha sonra bu Authorization Code ile Token Endpointe gidilerek Access Token alınır. Farklı firmaların client uygulamalarını kendi identity serverımız üzerinden kendi ApiResources'larımıza eriştirmek için üyelik sistemi kullanıyorsak tercih ettiğimizi bir altyapıdır. Net Core MVC gibi server-side taraflı çözümlerde tercih edilebilir.

Implicit Grant: Üyelik sistemi olan altyapılar için tercih edilir. Tek aşamalı doğrulamadan oluşur. Browserdan direkt olarak Authorize endpointe istek atılır ve Acess Token alınır. SPA ve Mobile uygulamalar için tercih edilen bir çözümdür. 

Resource Owner Credential Grant: Üyelik sistemi olan altyapılarda tercih edilir. Eğer şirket içi client-side ve server-side taraflı uygulamalar geliştiriyorsak, sadece client ve server taraflı çözümler bizim üzerimizde yaygınlanıyorsa tercih edilen yöntemdir. Bu durumda direk olarak Token Endpointe istek atılarak.Access Token alınabilir. Implicit Grant dan farkı ise birisinin token FrontChanneldan alması Resource Owner Credential Grant ise BackChanneldan almasıdır.

Client Credentials : Üyelik sistemi olmayan alt yapılarda tercih edilir. Client_Id ve Client_Secret ile sunucu ve istemci birbirleri ile güven dahilinde haberleşebilir. 

Authorize Endpointine istek attığımızda belirli return typelar verebiliriz. Bu return typelar code token code id_token cinsinden olabilir. Bu return typeları client tarafında verebiliyoruz. Code Authorize Code karşılık gelirken token ise Acess Token  karşılık gelir. Server side teknolojiler için Authorize Endpointinden access token direkt olarak dönülmesi doğru değildir. id_Token ise identity token anlamına gelir. Id Token İdentity Token kullanıcı sisteme girdikten sonra kullanıcıya ait kimlik bilgilerinin doğrulanmasını sağlayan JETON. Authrize EndPointten code, id_token ve token da istenebilir.Birden fazla token alma biçimine hibrid akış Hybrid Flow denir.En uygun olan server-side uygulamalar için code id_token ikilisidir.Id_Token yine bir Json Web Token fakat, içerisinde sadece oturum açan kullanıcı idsi barındırır.client tarafından doğru bir authorize code gelip gelmediğini anlamak için kullanılır. 


Proof Key for Code Exchange = Native clients (Mobile App, Akıllı saatlerde) veya SPA uygulamalarda kullanabilirsin. Bu tarz uygulamalar Client Secret değerini güvenli bir şekilde üzerinde tutamazlar.  Her bir istekde client_secret yerini tutacak code_challange ve code_verifier sayesinde güvenli bir şekilde identity server üzerinden bu istek gerçekleştirilebilir. Bu kodlar random bir şekilde oluşturuluyor. Client uygulaması random bir şekilde code_challange ve code_verifier oluşturur. Authorization Server istek yaparken bu code challange istek de taşınır. Authorization Server bu code_challange kendi üzerinde kaydeder ve client'a  auth token gönderir. Client bu sefer code_verifier ile birlikte authorization code gönderir. Auth server code_challenge ile code_verifier karşılaştırır eğer doğruysa access token alma endpointe gider. Merkezi bir üyelik sistemi kullanıyorsak bu yapıyı kullanmalıyız. (PKCE)


Kullanıcı hakkındaki ekstra openid dışındaki bilgileri UserInfo endpint üzerinden edineceğiz. Veriler cookie'de tutulduğundan cookie şişirmemek adına sadece gerekli bilgiler cookie eklendi. client tarafında api ile ilgili bir izin scope verildiğinde id_token refresh_token ve access_token gibi bilgilere'de HttpContext Properties üzerinden erişebiliriz.


Custom bir user entegrasyonu için TestUserStore yerine kendi Repository'lerimizden kullanıcı işlemlerini yaparız. Account Controller içerisinde ilgili yerleri güncelleriz.

IClientStore interface client bilgilerine erişmemizi sağlayan service

IAuthenticationSchemeProvider authenticaation şemamız ile ilgili özelliklere erişmemizi sağlayan servis
IEventService authentication işleminde event fırlatmamızı sağlayan servis
IIdentityServerInteractionService identity server ile iletişime geçmemizi sağlayan servis. IdentityServer context giriş çıkış yap deny ver gibi işlemleri yapabiliriz.

IProfile Service ile AddTestUsers yerine custom User Implementasyonunu Identity Server'a tanışmış oluyoruz.
Kullanıcı Login olduğunda hangi claimler token içerisine eklenecek. Bunu yapmak için IProfileService diye bir interfaceden implemente ederek bu işlemleri yapacağız.

Startup dosyasına AddProfileService olarak CustomProfile servisimizi ekliyoruz. Artık custom user claimler bu profile servis üzerinden oluşacaktır.

AddRequestedClaims methodu ile Token içerisine ekleyeceğiz claimleri RequestedProfileContext'e verdik.
IssuedClaims olarak da verirsek bu durumda UserInfo Endpointe gitmeden kullanıcıya ait claimleride Access Token içerisinde görüntülüyebiliriz. // json web token'a gömülür.
// Best Practice açısında AddRequestedClaims kullanarak => UserInfo EndPointe istek atarak almaktır.


Not: NameClaimType ile IdentityServer Startup dosyası üzerinden User.Identity.Name alanından name bilgisinin okunmasını sağlamış olduk.

// angular odic client library ihtiyaç var
