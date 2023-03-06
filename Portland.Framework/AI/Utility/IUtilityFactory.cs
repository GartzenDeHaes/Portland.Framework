using Portland.Text;
using Portland.Types;

namespace Portland.AI.Utility
{
	public interface IUtilityFactory
	{
		void Clear();
		UtilityFactory.AgentBuilder CreateAgent(string agentTypeName, string agentName);
		UtilitySet CreateAgentInstance(string agentTypeName, string name);
		UtilityFactory.AgentTypeBuilder CreateAgentType(string objectiveSetName, string agentTypeName);
		UtilityFactory.ConsiderationBuilder CreateConsideration(string objectiveName, in string propertyName);
		UtilityFactory.ObjectiveBuilder CreateObjective(string name);
		UtilityFactory.ObjectiveSetBuilder CreateObjectiveSetBuilder(string setName);
		void DefineAlertForPropertyDefinition(in string propertyId, PropertyDefinition.AlertType type, in Variant8 value, string flagName);
		void DestroyInstance(string name);
		PropertyValue GetGlobalProperty(in string name);
		bool HasGlobalPropertyDefinition(in string propName);
		bool HasObjective(string objectiveName);
		bool HasObjectiveSet(string name);
		bool HasPropertyDefinition(in string propName);
		void ParseLoad(string xml);
		void ParseLoad(XmlLex xml);
		void TickAgents();
	}
}