'use strict';
var base_url = $('body').data('sl')+"/"; var ssl = $('body').data('ssl'); var token=$('body').data('cus'); var cus=$('body').data('sectionid'); var tkusr=$('body').data('tkusr');
angular.module('scmr.svc', [])
.factory('promiseFactory', function($q) {
  return {
    decorate: function(promise) {
      promise.success = function(callback) {
        promise.then(callback);

        return promise;
      };

      promise.error = function(callback) {
        promise.then(null, callback);

        return promise;
      };
    },
    defer: function() {
      var deferred = $q.defer();

      this.decorate(deferred.promise);

      return deferred;
    }
  };
})
.factory('userS', function () {
        var fac = {};
        fac.CurrentUser = null;
        fac.SetCurrentUser = function (user) {
            fac.CurrentUser = user;
            sessionStorage.user = angular.toJson(user);
        };
        fac.GetCurrentUser = function () {
            fac.CurrentUser = angular.fromJson(sessionStorage.user);
            return fac.CurrentUser;
        };
        return fac;
    })
.factory("vsom", function($http,promiseFactory,userS) {
   
        var auth = function(md) {var dfd = promiseFactory.defer(); 
        	 $http({ method: "post", url: base_url + "Auth", data: { user: md.ident, pass:md.pass},headers: { 'Content-Type': 'application/x-www-form-urlencoded' }})
        	 .then(function(result){ return dfd.resolve(result); },function(err) { return dfd.reject(err);}); return dfd.promise; };
       var authtk= function(md,tok){
       	var dfd = promiseFactory.defer(); 
       	var obj = { 'username': md, 'password': tok, 'grant_type': 'password' };
            Object.toparams = function ObjectsToParams(obj) {var p = []; for (var key in obj) {p.push(key + '=' + encodeURIComponent(obj[key]));} return p.join('&');};
      $http({ method: "post", url: ssl, data: Object.toparams(obj),headers: { 'Content-Type': 'application/x-www-form-urlencoded' }})
        	 .then(function(result){userS.SetCurrentUser(result.data);  return dfd.resolve(result); },function(err) { return dfd.reject(err);}); return dfd.promise;
            
       };
    var autdtk=function(){userS.CurrentUser = null;userS.SetCurrentUser(userS.CurrentUser);};    
    var authtoken=function(md) {var dfd = promiseFactory.defer(); 
    	$http({ method: "post", url: base_url + "Token", data: { user: md.ident, pass:md.pass,access:md.access},headers: { 'Content-Type': 'application/x-www-form-urlencoded' }})
    	.then(function(result){ return dfd.resolve(result); },function(err) { return dfd.reject(err);}); return dfd.promise; };
   
   var searchbl=function(md) {var dfd = promiseFactory.defer(); 
    	$http({ method: "get", url: ssl+'Doc/?bl='+md+'&type=quote&tok='+token, data: {},headers: { 'Content-Type': 'application/x-www-form-urlencoded' }})
    	.then(function(result){ return dfd.resolve(result); },function(err) { return dfd.reject(err);}); return dfd.promise; };
     
     var searchbl0=function(md) {var dfd = promiseFactory.defer(); 
    	$http({ method: "post", url: base_url + "Api", data: { user: md},headers: { 'Content-Type': 'application/x-www-form-urlencoded' }})
    	.then(function(result){ return dfd.resolve(result); },function(err) { return dfd.reject(err);}); return dfd.promise; };
     
    var blinvoices=function(md) {var dfd = promiseFactory.defer(); 
    	$http({ method: "get", url: ssl+'Doc/?bl='+md+'&type=inv&tok='+token, data: {},headers: { 'Content-Type': 'application/x-www-form-urlencoded' }})
    	.then(function(result){ return dfd.resolve(result); },function(err) { return dfd.reject(err);}); return dfd.promise; };
    
    
    var quotation =function(md) {var dfd = promiseFactory.defer(); 
    	$http({ method: "get", url: ssl+'Quotation/?bl='+md.numbl+'&chassis='+md.numchassis+'&date='+md.date+'&type=quote&tok='+token+'&owner='+cus+'&tkusr='+tkusr, data: {},headers: { 'Content-Type': 'application/x-www-form-urlencoded' }})
    	.then(function(result){ return dfd.resolve(result); },function(err) { return dfd.reject(err);}); return dfd.promise; };
   
    var validquotation = function(md,el) {var dfd = promiseFactory.defer(); 
        	 $http({ method: "post", url: base_url + "Quotation", data: { bl: md.numbl, chassis:md.numchassis,date:md.date,lignes:el},headers: { 'Content-Type': 'application/x-www-form-urlencoded' }})
        	 .then(function(result){ return dfd.resolve(result); },function(err) { return dfd.reject(err);}); return dfd.promise; };
     
     var valquotationssl =function(md,ref,el,acces) {var dfd = promiseFactory.defer(); 
    	$http({ method: "get", url: ssl+'Quotation/?bl='+md.numbl+'&chassis='+md.numchassis+'&date='+md.date+'&type=inv&tok='+ref+'&owner='+cus+'&tkusr='+tkusr, data: {},headers: { 'Content-Type': 'application/x-www-form-urlencoded' }})
    	.then(function(result){ return dfd.resolve(result); },function(err) { return dfd.reject(err);}); return dfd.promise; };
    	  	 
  var recap = function(md) {var dfd = promiseFactory.defer(); 
        	 $http({ method: "get", url: ssl + "Recap/?owner="+cus+"&from="+md.start+"&to="+md.end, data: {},headers: { 'Content-Type': 'application/x-www-form-urlencoded' }})
        	 .then(function(result){ return dfd.resolve(result); },function(err) { return dfd.reject(err);}); return dfd.promise; };
        	       	  	 	
 return { hack: auth ,inside:authtk,outside:autdtk, token : authtoken, Bl : searchbl, quote : quotation , goquote : valquotationssl , finances :blinvoices ,requests:recap};
 
});