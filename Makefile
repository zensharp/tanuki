.DEFAULT_GOAL := cq

cq: ## Show all Makefile targets
	cd Tanuki && dotnet watch -- html TestFiles/codeclimate.json

