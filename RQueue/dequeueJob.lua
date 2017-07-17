local dequeuedJob = redis.call("LPOP",KEYS[1]);
if dequeuedJob == false then
	 return
end
local jobIdStartIndex = string.find(dequeuedJob,"%d+");
local startingMatch = string.sub(dequeuedJob,jobIdStartIndex);
local jobIdEndIndex = string.find(startingMatch,"\"") - 1;
local jobId = string.sub(startingMatch,0,jobIdEndIndex);
redis.call("HSET",KEYS[2],jobId,dequeuedJob);
return dequeuedJob;