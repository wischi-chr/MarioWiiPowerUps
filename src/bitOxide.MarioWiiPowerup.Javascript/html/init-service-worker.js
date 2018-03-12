function init_serviceworker()
{
	if ('serviceWorker' in navigator) {
		navigator.serviceWorker.register('/service-worker.js',{
			scope: '.'
		});
	}
}
