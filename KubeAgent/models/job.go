package models

type OrchestrationJob struct {
	Id          string `json:"id"`
	NamespaceId string `json:"namespaceId"`
	Action      string `json:"action"`
	TargetId    string `json:"targetId"`
}
