let now = $state(Date.now());

if (typeof window !== 'undefined') {
	setInterval(() => {
		now = Date.now();
	}, 1000);
}

export function currentTime(): number {
	return now;
}
