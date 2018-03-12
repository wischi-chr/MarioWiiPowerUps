var CACHE = 'cache-and-update';

self.addEventListener('install', function(evt) {
  console.log('The service worker is being installed.');
  evt.waitUntil(precache());
});

self.addEventListener('fetch', function(evt) {
  console.log('The service worker is serving the asset.');
  evt.respondWith(fromCache(evt.request));
  evt.waitUntil(update(evt.request));
});

function precache() {
  return caches.open(CACHE).then(function (cache) {
    return cache.addAll([
	  './init-service-worker.js',
      './bridge.min.js',
	  './bridge.meta.min.js',
      './imgscale.js',
	  './index.html',
	  './manifest.json',
	  './bitOxide.MarioWiiPowerup.Javascript.min.js',
	  './img/bg.png',
	  './img/bowser.png',
	  './img/bullet.png',
	  './img/fireflower.png',
	  './img/fly.png',
	  './img/iceflower.png',
	  './img/mini.png',
	  './img/minibowser.png',
	  './img/mushroom.png',
	  './img/penguin.png',
	  './img/pow.png',
	  './img/questionmark.png',
	  './img/star.png'	  
    ]);
  });
}

function fromCache(request) {
  return caches.open(CACHE).then(function (cache) {
    return cache.match(request).then(function (matching) {
      return matching || Promise.reject('no-match');
    });
  });
}

function update(request) {
  return caches.open(CACHE).then(function (cache) {
    return fetch(request).then(function (response) {
      return cache.put(request, response);
    });
  });
}