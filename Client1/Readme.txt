Client uygulamalar Authorization Server üzerinden Access Token alıp bu bilgileri Cookie Based olarak tutacakları için Open Id Connect paketini yüklememiz gerekir. Auth2.0 server-side taraftaki Hesapların yönetimi ve Access Token dağıtımından sorumludur.

Uygulamamızda birden fazla üyelik sistemi varsa bu üyelik sistemlerini birbirinden ayrıştırabilmek için scheme kavramını kullanıyoruz.

// Microsoft.AspNetCore.Authentication.OpenIdConnect paketi ile identity server üzerinden kimlik doğrulama yapabiliriz.