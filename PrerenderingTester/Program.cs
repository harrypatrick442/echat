using FilesRelayPrerendering;
using Shutdown;
ShutdownManager.Initialize((exitCode) => { }, ()=>null);
FilesRelayPrerenderingHelper.Prerender();