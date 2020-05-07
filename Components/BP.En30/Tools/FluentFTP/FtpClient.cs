System.InvalidOperationException: 堆栈为空。
   在 System.ThrowHelper.ThrowInvalidOperationException(ExceptionResource resource)
   在 System.Collections.Generic.Stack`1.Peek()
   在 ...()
   在 ..(Int32 targetOffset, Boolean dropContent)
   在 ..(Int32 offset, Int32 end)
   在 ..(IMethodDeclaration mD, IMethodBody mB, Boolean handleExpressionStack)
   在 ..(IMethodDeclaration mD, IMethodBody mB)
   在 ..(IMethodDeclaration value)
   在 ..(IMethodDeclarationCollection methods)
   在 ..(ITypeDeclaration value)
   在 ..TranslateTypeDeclaration(ITypeDeclaration value, Boolean memberDeclarationList, Boolean methodDeclarationBody)
   在 ..(ITypeDeclaration typeDeclaration, String sourceFile, ILanguageWriterConfiguration languageWriterConfiguration)
namespace FluentFTP
{
}

