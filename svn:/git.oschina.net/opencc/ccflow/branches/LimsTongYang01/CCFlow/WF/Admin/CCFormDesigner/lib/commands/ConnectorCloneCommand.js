/** 
* This command just clones an existing {Connector}. All it needs is an id of
* cloned {Connector}
* @this {ConnectorCloneCommand} 
* @constructor
* @param {Number} parentConnectorId - the Id of parent {Connector}
* @author dgq
*/
function ConnectorCloneCommand(parentConnectorId) {
    this.oType = 'ConnectorCloneCommand';

    this.firstExecute = true;

    /**This will keep the newly created  Connector id*/
    this.connectorId = null;

    /**This keeps the cloned connector Id*/
    this.parentConnectorId = parentConnectorId;
}


ConnectorCloneCommand.prototype = {

    /**This method got called every time the Command must execute*/
    execute: function () {
        if (this.firstExecute) {
            //get old connector and clone it
            var createdConnector = CONNECTOR_MANAGER.connectorGetById(this.parentConnectorId).clone();
            
            //move it 10px low and 10px right
            createdConnector.transform(Matrix.translationMatrix(10, 10));
            
            //store newly created connector id
            this.connectorId = createdConnector.id;
            //update diagram state
            selectedConnectorId = this.connectorId;
            setUpEditPanel(createdConnector);
            state = STATE_FIGURE_SELECTED;

            this.firstExecute = false;
        }
        else { //redo
            throw "Not implemented";
        }
    },

    /**This method should be called every time the Command should be undone*/
    undo: function () {
        ConnectorManager.connectorRemoveById(this.connectorId, true);
        state = STATE_NONE;
    }
}