const getEndpointFromArgs = () => {
  let endpointArgument = process.argv[2];
  return endpointArgument.substring("--endpoint=".length);
};

module.exports = getEndpointFromArgs;
