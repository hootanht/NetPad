import {CompletionItem as OmnisharpCompletionItem, CompletionRequest, IOmniSharpService, ISession} from "@domain";
import * as monaco from "monaco-editor";
import {CancellationToken, editor, IRange, languages} from "monaco-editor";
import ISingleEditOperation = editor.ISingleEditOperation;
import MonacoCompletionItem = languages.CompletionItem;
import MonacoCompletionContext = languages.CompletionContext;
import CompletionItemKind = languages.CompletionItemKind;
import MonacoCompletionItemKind = languages.CompletionItemKind;

export class OmnisharpCompletionProvider {
    constructor(
        @ISession private readonly session: ISession,
        @IOmniSharpService private readonly omnisharpService: IOmniSharpService) {
    }

    public register() {
        monaco.languages.registerCompletionItemProvider("csharp", {
            triggerCharacters: ["."],
            provideCompletionItems: async (model, position, ctx, token) => {
                const word = model.getWordUntilPosition(position);
                const range = {
                    startLineNumber: position.lineNumber,
                    endLineNumber: position.lineNumber,
                    startColumn: word.startColumn,
                    endColumn: word.endColumn
                };

                return {
                    suggestions: await this.createCompletionItems(range, ctx, token)
                };
            },
            resolveCompletionItem: async (item: languages.CompletionItem, token: CancellationToken) => {
                const resolution = await this.omnisharpService.getCompletionResolution(this.session.active.script.id, <any>item);

                const resItem = resolution.item;

                if (resItem.detail) {
                    item.detail = resItem.detail;
                }

                if (resItem.documentation) {
                    item.documentation = resItem.documentation;
                }

                return item;
            }
        });
    }

    private async createCompletionItems(range: IRange, ctx: MonacoCompletionContext, token: CancellationToken): Promise<MonacoCompletionItem[]> {
        const request = new CompletionRequest();
        request.line = range.startLineNumber - 1;
        request.column = range.endColumn - 1;
        request.completionTrigger = (ctx.triggerKind + 1) as any;
        request.triggerCharacter = ctx.triggerCharacter;

        if (token.isCancellationRequested) {
            return [];
        }

        const omnisharpCompletions = await this.omnisharpService.getCompletion(this.session.active.script.id, request);

        if (token.isCancellationRequested || !omnisharpCompletions || !omnisharpCompletions.items) {
            return [];
        }

        const suggestions = omnisharpCompletions.items
            .map(omnisharpCompletion => this.convertToMonacoCompletionItem(range, omnisharpCompletion));

        if (token.isCancellationRequested) {
            return [];
        }

        return suggestions;
    }

    private convertToMonacoCompletionItem(range: IRange, omnisharpCompletion: OmnisharpCompletionItem) {
        const kind = MonacoCompletionItemKind[omnisharpCompletion.kind];

        const newText = omnisharpCompletion.textEdit?.newText ?? omnisharpCompletion.label;

        const insertText = omnisharpCompletion.insertTextFormat === "Snippet"
            ? newText // TODO might need to convert to a monaco compatible snippet
            : newText;

        let sortText = omnisharpCompletion.sortText;
        if (kind === CompletionItemKind.Property)
            sortText = "a" + sortText;
        else if (kind === CompletionItemKind.Method)
            sortText = "b" + sortText;
        else
            sortText = "c" + sortText;

        // We don't want space to be a commit character for the suggestion, its annoying when one is typing
        // a variable name 'list' and hits space that the suggestion will be selected
        const commitCharacters = omnisharpCompletion.commitCharacters.filter(c => c != " ");

        const docs = omnisharpCompletion.documentation ? {
            value: omnisharpCompletion.documentation,
            isTrusted: false,
            supportHtml: true,
            supportThemeIcons: true
        } : undefined;

        const additionalTextEdits = omnisharpCompletion.additionalTextEdits?.map(ate => {
            return <ISingleEditOperation>{
                text: ate.newText,
                range: {
                    startLineNumber: ate.startLine,
                    startColumn: ate.startColumn,
                    endLineNumber: ate.endLine,
                    endColumn: ate.endColumn
                }
            };
        });

        const tags = omnisharpCompletion.tags && omnisharpCompletion.tags[0] === "Deprecated" ? 1 : [];

        return <MonacoCompletionItem>{
            label: omnisharpCompletion.label,
            detail: omnisharpCompletion.detail,
            kind: kind,
            documentation: docs,
            commitCharacters: commitCharacters,
            preselect: omnisharpCompletion.preselect,
            filterText: omnisharpCompletion.filterText,
            insertText: insertText,
            range: range,
            tags: tags,
            sortText: sortText,
            additionalTextEdits: additionalTextEdits,
            command: omnisharpCompletion.hasAfterInsertStep ? {
                id: "",
                command: "csharp.completion.afterInsert",
                title: "",
                arguments: [omnisharpCompletion]
            } : undefined
        };
    }
}
