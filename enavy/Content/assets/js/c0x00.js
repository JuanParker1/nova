var base_url = $('body').data('baseurl')+"/";
var sectionid=$('body').data('sectionid');
var app = angular.module('scmr.ctrl', ['ui.materialize','scmr.svc']);
app.controller('WelcomeCtrl',function ($scope,$http,$timeout,vsom) {
	$scope.screendata={'btnenter':'Entrer','step':0,'message':'' , 'showvin':true , 'progress' : false};
	$scope.md={}; 
	$scope.process=function(){ $scope.screendata.message='';
		if($scope.md.ident===undefined || $scope.md.pass===undefined)
		{
			$scope.screendata.message='Veuillez saisir des valeurs';
		}
		if($scope.md.ident!=undefined && $scope.md.pass!=undefined && $scope.screendata.showvin==true)
		{
			$scope.screendata.progress=true;
			var req=vsom.hack($scope.md);
		    req.success(function(result) { 
		 	 if(result.data.status==1){ /*window.location.href=result.data.lnk;*/$scope.screendata.showvin=false;
		 	 var rm=vsom.inside(result.data.tkey,result.data.tkeyusr); $scope.screendata.message='Connexion au serveur en cours...';
		 	 rm.success(function(resul){  $scope.screendata.progress=false; 
		 	 	if(resul.status==200){window.location.href=result.data.lnk;}else{$scope.screendata.message='La demande a échoué.';}})
		 	 .error(function(er){  $scope.screendata.progress=false;});
		 	 }
		 	 if(result.data.status==2){$scope.screendata.message='Compte indisponible'; $scope.screendata.progress=false;}
		 	 if(result.data.status==0){$scope.screendata.message='Identifiant / mot de passe incohérent'; $scope.screendata.progress=false;}
		 	 if(result.data.status==3){$scope.screendata.message='La négotiation du canal avec le serveur à échoué'; $scope.screendata.progress=false;}
		    }).error(function(err){$scope.screendata.progress=false;$scope.screendata.message='Demande non effectuée!';});
		}
		if($scope.md.ident!=undefined && $scope.md.pass!=undefined && $scope.screendata.showvin==false && $scope.md.access!=undefined)
		{   var req=vsom.token($scope.md); 
			req.success(function(result) { if(result.data.status==1)window.location.href=result.data.lnk;else $scope.md.access=undefined;})
			.error(function(err){$scope.screendata.progress=false;$scope.screendata.message='Demande non effectuée!';});
		}
	}; 
})
.controller('EtsCtrl',function ($scope,$http,$timeout) {
$scope.screendata={bl:'',blconsigne:'',address:'',phone:'', el:[], mttc:'0',mht:'0',mtva:'0' , chassis:'', showvin : false , vinlib:'',actbtn:'indigo darken-3'};
	$scope.md={};
});