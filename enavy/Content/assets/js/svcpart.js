'use strict';
var base_url = $('body').data('baseurl')+"/"; var ssl = $('body').data('ssl'); var ss = $('body').data('ss'); var token=$('body').data('cus'); var cus=$('body').data('sectionid');
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
    
   var searchbl=function(md) {var dfd = promiseFactory.defer(); 
    	$http({ method: "get", url: ssl+'Vin/?bl='+md+'&type=obl&vin=null', data: {},headers: { 'Content-Type': 'application/x-www-form-urlencoded' }})
    	.then(function(result){ return dfd.resolve(result); },function(err) { return dfd.reject(err);}); return dfd.promise; };
      var authtk= function(md,tok){
       	var dfd = promiseFactory.defer(); 
       	var obj = { 'username': md, 'password': tok, 'grant_type': 'password' };
            Object.toparams = function ObjectsToParams(obj) {var p = []; for (var key in obj) {p.push(key + '=' + encodeURIComponent(obj[key]));} return p.join('&');};
      $http({ method: "post", url: ss, data: Object.toparams(obj),headers: { 'Content-Type': 'application/x-www-form-urlencoded' }})
        	 .then(function(result){userS.SetCurrentUser(result.data);  return dfd.resolve(result); },function(err) { return dfd.reject(err);}); return dfd.promise;
            
       };
         var auth = function(tk,tp,vin) {var dfd = promiseFactory.defer(); 
        	 $http({ method: "post", url: base_url + "Authp", data: { user: tk, pass:tp, cte:vin},headers: { 'Content-Type': 'application/x-www-form-urlencoded' }})
        	 .then(function(result){ return dfd.resolve(result); },function(err) { return dfd.reject(err);}); return dfd.promise; };
       
    var chkv =function(bl,vin) {var dfd = promiseFactory.defer(); 
    	$http({ method: "get", url: ssl+'Vin/?bl='+bl+'&type=ovin&vin='+vin, data: {},headers: { 'Content-Type': 'application/x-www-form-urlencoded' }})
    	.then(function(result){ return dfd.resolve(result); },function(err) { return dfd.reject(err);}); return dfd.promise; };
     	  	 	
 return { Bl : searchbl, Vin : chkv ,inside:authtk,hack: auth };
 
});

//etape : recherche de bl --> choix chassis --> (ouverture screen:  rappel info bl)--> date sortie --> cotation --> impression 
