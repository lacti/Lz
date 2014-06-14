all:
	svn update \
		&& xbuild Lz.sln /p:Configuration=Server \
		&& (screen -S lzserver -X quit || true) \
		&& screen -dmS lzserver ./start_server.sh

server:
	xbuild Lz.sln /p:Configuration=Server
