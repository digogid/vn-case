let userIP = '';

function enviarDados() {
  const dados = {
    Browser: detectarBrowser(),
    Name: document.title,
    IP: userIP || "Não detectado",
    Input: JSON.stringify(capturarInputs())
  };

  console.log(dados);
}


function capturarInputs() {
  const inputs = document.querySelectorAll("input");
  let form = {};
  for(const input of inputs) {
    form[input.name] = input.value;
  }
  return form;
}

function getIP(json) {
  userIP = json.ip;
}

function detectarBrowser() {
  var isOpera =
    (!!window.opr && !!opr.addons) ||
    !!window.opera ||
    navigator.userAgent.indexOf(" OPR/") >= 0;

  var isFirefox = typeof InstallTrigger !== "undefined";

  var isSafari =
    /constructor/i.test(window.HTMLElement) ||
    (function(p) {
      return p.toString() === "[object SafariRemoteNotification]";
    })(
      !window["safari"] ||
        (typeof safari !== "undefined" && safari.pushNotification)
    );

  var isIE = /*@cc_on!@*/ false || !!document.documentMode;

  var isEdge = !isIE && !!window.StyleMedia;

  var isChrome = !!window.chrome;

  var isEdgeChromium = isChrome && navigator.userAgent.indexOf("Edg") != -1;

  
  if (isOpera) return "Opera";
  if (isFirefox) return "Firefox";
  if (isSafari) return "Safari";
  if (isEdgeChromium) return "Edge-Chromium";
  if (isChrome) return "Chrome";
  if (isEdge) return "Edge";
  if (isIE) return "Internet Explorer";

  return "Indefinido";
}
